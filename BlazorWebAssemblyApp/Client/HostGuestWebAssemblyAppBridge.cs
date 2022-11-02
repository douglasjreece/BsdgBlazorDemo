using BlazorWebAssemblyApp.Shared;
using GameComponents;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorWebAssemblyApp
{
    public class HostGuestWebAssemblyAppBridge : IHostGuestBridge
    {
        public HostGuestWebAssemblyAppBridge(IHubConnectionBuilder hubBuilder)
        {
            hubConnection = hubBuilder.Build();
        }

        private readonly HubConnection hubConnection;

        public IGuestConnection CreateConnectionToGuest(string gameId, PlayerColor hostColor, Func<char, int, Task> onGuestPlay, Func<Task> onGuestPass)
        {
            return new GuestConnection(gameId, hostColor, onGuestPlay, onGuestPass, hubConnection);
        }

        public IHostConnection CreateConnectionToHost(string gameId, PlayerColor guestColor, Func<char, int, Task> onHostPlay, Func<Task> onHostPass, Func<Task> onHostNewGame)
        {
            return new HostConnection(gameId, guestColor.Opposite(), onHostPlay, onHostPass, onHostNewGame, hubConnection);
        }

        class GuestConnection : IGuestConnection
        {
            private readonly string gameId;
            private readonly PlayerColor hostColor;
            private readonly Func<char, int, Task> onGuestPlay;
            private readonly Func<Task> onGuestPass;
            private readonly HubConnection hubConnection;
            bool hubConnected;
            bool guestConnected;

            public GuestConnection(string gameId, PlayerColor hostColor, Func<char, int, Task> onGuestPlay, Func<Task> onGuestPass, HubConnection hubConnection)
            {
                this.gameId = gameId;
                this.hostColor = hostColor;
                this.onGuestPlay = onGuestPlay;
                this.onGuestPass = onGuestPass;
                this.hubConnection = hubConnection;
            }

            public async Task<bool> ConnectToGuest()
            {
                if (!hubConnected)
                    try
                    {
                        if (hubConnection.State != HubConnectionState.Connected)
                        {
                            await hubConnection.StartAsync();
                        }
                        await hubConnection.InvokeAsync(HubCommand.RegisterHost, gameId, hostColor);

                        hubConnection.On(HubCommand.GuestConnected, GuestConnected);
                        hubConnection.On(HubCommand.GuestPassMade, onGuestPass);
                        hubConnection.On(HubCommand.GuestPlayMade, onGuestPlay);

                        hubConnected = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return false;
                    }

                return guestConnected;
            }

            async Task GuestConnected()
            {
                if (!guestConnected)
                {
                    await hubConnection.InvokeAsync(HubCommand.HostConnected, gameId, hostColor);
                    guestConnected = true;
                }
            }

            public void Dispose()
            {
                Task.Run(async () =>
                {
                    if (hubConnection is not null && hubConnection.State == HubConnectionState.Connected)
                    {
                        await hubConnection.StopAsync();
                    }
                });
            }

            public async Task HostPassMade()
            {
                await hubConnection.InvokeAsync(HubCommand.HostPassMade, gameId, hostColor);
            }

            public async Task HostPlayMade(char x, int y)
            {
                await hubConnection.InvokeAsync(HubCommand.HostPlayMade, gameId, hostColor, x, y);
            }

            public async Task HostNewGame()
            {
                await hubConnection.InvokeAsync(HubCommand.HostNewGame, gameId, hostColor);
            }
        }

        class HostConnection : IHostConnection
        {
            private readonly string gameId;
            private readonly PlayerColor hostColor;
            private readonly Func<char, int, Task> onHostPlay;
            private readonly Func<Task> onHostPass;
            private readonly Func<Task> onHostNewGame;
            private readonly HubConnection hubConnection;
            bool hubConnected;
            bool hostConnected;

            public HostConnection(string gameId, PlayerColor hostColor, Func<char, int, Task> onHostPlay, Func<Task> onHostPass, Func<Task> onHostNewGame, HubConnection hubConnection)
            {
                this.gameId = gameId;
                this.hostColor = hostColor;
                this.onHostPlay = onHostPlay;
                this.onHostPass = onHostPass;
                this.onHostNewGame = onHostNewGame;
                this.hubConnection = hubConnection;
            }

            public async Task<bool> ConnectToHost()
            {
                if (!hubConnected)
                    try
                    {
                        if (hubConnection.State != HubConnectionState.Connected)
                        {
                            await hubConnection.StartAsync();
                        }
                        await hubConnection.InvokeAsync(HubCommand.RegisterGuest, gameId, hostColor);

                        hubConnection.On(HubCommand.HostConnected, HostConnected);
                        hubConnection.On(HubCommand.HostPassMade, onHostPass);
                        hubConnection.On(HubCommand.HostPlayMade, onHostPlay);
                        hubConnection.On(HubCommand.HostNewGame, onHostNewGame);

                        hubConnected = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return false;
                    }

                return hostConnected;
            }

            public void Dispose()
            {
                Task.Run(async () =>
                {
                    if (hubConnection is not null && hubConnection.State == HubConnectionState.Connected)
                    {
                        await hubConnection.StopAsync();
                    }
                });
            }

            async Task HostConnected()
            {
                if (!hostConnected)
                {
                    await hubConnection.InvokeAsync(HubCommand.GuestConnected, gameId, hostColor);
                    hostConnected = true;
                }
            }

            public async Task GuestPassMade()
            {
                await hubConnection.InvokeAsync(HubCommand.GuestPassMade, gameId, hostColor);
            }

            public async Task GuestPlayMade(char x, int y)
            {
                await hubConnection.InvokeAsync(HubCommand.GuestPlayMade, gameId, hostColor, x, y);
            }

            public async Task GuestReady()
            {
                await hubConnection.InvokeAsync(HubCommand.GuestConnected, gameId, hostColor);
            }
        }

    }
}
