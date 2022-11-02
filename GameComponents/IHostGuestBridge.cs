namespace GameComponents
{
    public interface IHostGuestBridge
    {
        IHostConnection CreateConnectionToHost(string gameId, PlayerColor guestColor, Func<char, int, Task> onHostPlay, Func<Task> onHostPass, Func<Task> onHostNewGame);
        IGuestConnection CreateConnectionToGuest(string gameId, PlayerColor hostColor, Func<char, int, Task> onGuestPlay, Func<Task> onGuestPass);
    }
}
