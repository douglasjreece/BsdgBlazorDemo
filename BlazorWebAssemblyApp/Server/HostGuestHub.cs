using BlazorWebAssemblyApp.Shared;
using GameComponents;
using Microsoft.AspNetCore.SignalR;

namespace BlazorWebAssemblyApp.Server
{
    public class HostGuestHub : Hub<IHostGuestHub>
    {
        public async Task RegisterHost(string gameId, PlayerColor hostColor)
        {
            string groupName = $"{gameId}-{hostColor}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).HostNewGame();
            await Clients.Group(groupName).HostConnected();
        }

        public async Task RegisterGuest(string gameId, PlayerColor hostColor)
        {
            string groupName = $"{gameId}-{hostColor}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).GuestConnected();
        }

        public async Task HostConnected(string gameId, PlayerColor hostColor)
        {
            string groupName = $"{gameId}-{hostColor}";
            await Clients.Group(groupName).HostConnected();
        }

        public async Task GuestConnected(string gameId, PlayerColor hostColor)
        {
            string groupName = $"{gameId}-{hostColor}";
            await Clients.Group(groupName).GuestConnected();
        }

        public async Task HostPassMade(string gameId, PlayerColor hostColor)
        {
            string groupName = $"{gameId}-{hostColor}";
            await Clients.Group(groupName).HostPassMade();
        }

        public async Task HostPlayMade(string gameId, PlayerColor hostColor, char x, int y)
        {
            string groupName = $"{gameId}-{hostColor}";
            await Clients.Group(groupName).HostPlayMade(x, y);
        }

        public async Task HostNewGame(string gameId, PlayerColor hostColor)
        {
            string groupName = $"{gameId}-{hostColor}";
            await Clients.Group(groupName).HostNewGame();
        }

        public async Task GuestPassMade(string gameId, PlayerColor hostColor)
        {
            string groupName = $"{gameId}-{hostColor}";
            await Clients.Group(groupName).GuestPassMade();
        }

        public async Task GuestPlayMade(string gameId, PlayerColor hostColor, char x, int y)
        {
            string groupName = $"{gameId}-{hostColor}";
            await Clients.Group(groupName).GuestPlayMade(x, y);
        }
    }
}
