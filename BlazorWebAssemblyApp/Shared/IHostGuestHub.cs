namespace BlazorWebAssemblyApp.Shared
{
    public interface IHostGuestHub
    {
        Task RegisterHost();
        Task RegisterGuest();
        Task HostConnected();
        Task HostPassMade();
        Task HostPlayMade(char x, int y);
        Task HostNewGame();
        Task GuestConnected();
        Task GuestPassMade();
        Task GuestPlayMade(char x, int y);
    }
}
