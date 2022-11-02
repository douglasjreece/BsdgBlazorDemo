using GameLogic.Othello;

namespace GameComponents
{
    public class LocalPlayer : IPlayer
    {
        private readonly OthelloGame game;
        private readonly Types.Color color;

        public LocalPlayer(OthelloGame game, Types.Color color)
        {
            this.game = game;
            this.color = color;
            game.PlayerReady(Types.Player.Of(color));
        }

        public string Description => "";

        public bool IsInteractive => true;

        public bool IsLocal => true;

        public bool CanResetGame => true;

        public void Dispose()
        {
            // do nothing
        }

        public async Task MakePlay(char x, int y)
        {
            await game.MakePlay(Types.Square.At(x, y));
        }

        public async Task MakePlay(Types.Position position)
        {
            await game.MakePlay(position.Square);
        }

        public async Task Pass()
        {
            await game.MakePlay(null);
        }
    }
}
