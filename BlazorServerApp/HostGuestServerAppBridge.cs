using GameComponents;

namespace BlazorServerApp
{
    public class HostGuestServerAppBridge : IHostGuestBridge
    {
        public IGuestConnection CreateConnectionToGuest(string gameId, PlayerColor hostColor, Func<char, int, Task> onGuestPlay, Func<Task> onGuestPass)
        {
            lock (relays)
            {
                Key key = new(gameId, hostColor);
                Relay? relay;
                if (!relays.TryGetValue(key, out relay))
                {
                    relay = new Relay();
                }
                relay.OnGuestPlay = onGuestPlay;
                relay.OnGuestPass = onGuestPass;
                if (relay.OnHostNewGame is not null)
                {
                    relay.OnHostNewGame.Invoke();
                }
                relays[key] = relay;    
                return relay;
            }
        }

        public IHostConnection CreateConnectionToHost(string gameId, PlayerColor guestColor, Func<char, int, Task> onHostPlay, Func<Task> onHostPass, Func<Task> onHostNewGame)
        {
            lock (relays)
            {
                Key key = new(gameId, guestColor.Opposite());
                Relay? relay;
                if (!relays.TryGetValue(key, out relay))
                {
                    relay = new Relay();
                }
                relay.OnHostPlay = onHostPlay;
                relay.OnHostPass = onHostPass;
                relay.OnHostNewGame = onHostNewGame;
                relays[key] = relay;
                return relay;
            }
        }

        public class Relay : IHostConnection, IGuestConnection
        {
            public Func<char, int, Task>? OnGuestPlay { get; set; }
            public Func<Task>? OnGuestPass { get; set; }
            public Func<char, int, Task>? OnHostPlay { get; set; }
            public Func<Task>? OnHostPass { get; set; }
            public Func<Task>? OnHostNewGame { get; set; }

            public async Task<bool> ConnectToGuest()
            {
                return await Task.FromResult(OnHostPlay is not null);
            }

            public async Task<bool> ConnectToHost()
            {
                return await Task.FromResult(OnGuestPlay is not null);
            }

            public void Dispose()
            {
                //
            }

            public async Task GuestPassMade()
            {
                OnGuestPass?.Invoke();
                await Task.CompletedTask;
            }

            public async Task GuestPlayMade(char x, int y)
            {
                OnGuestPlay?.Invoke(x, y);
                await Task.CompletedTask;
            }

            public async Task HostPassMade()
            {
                OnHostPass?.Invoke();
                await Task.CompletedTask;
            }

            public async Task HostPlayMade(char x, int y)
            {
                OnHostPlay?.Invoke(x, y);
                await Task.CompletedTask;
            }

            public async Task HostNewGame()
            {
                OnHostNewGame?.Invoke();
                await Task.CompletedTask;
            }

            public async Task GuestReady()
            {
                await Task.CompletedTask;
            }
        }


        record Key(string GameId, PlayerColor Color);

        readonly Dictionary<Key, Relay> relays = new();
    }
}
