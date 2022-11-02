namespace GameComponents
{
    public interface IHostConnection : IDisposable
    {
        Task<bool> ConnectToHost();
        Task GuestPlayMade(char x, int y);
        Task GuestPassMade();
        Task GuestReady();
    }
}
