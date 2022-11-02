namespace GameComponents
{
    public interface IGuestConnection : IDisposable
    {
        Task<bool> ConnectToGuest();
        Task HostPlayMade(char x, int y);
        Task HostPassMade();
        Task HostNewGame();
    }
}
