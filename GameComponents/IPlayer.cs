using GameLogic.Othello;

namespace GameComponents
{
    public interface IPlayer : IDisposable
    {
        string Description { get; }
        bool IsInteractive { get; }
        bool IsLocal { get; }
        bool CanResetGame { get; }
        Task MakePlay(char x, int y);
        Task MakePlay(Types.Position position);
        Task Pass();
    }
}
