using GameLogic.Othello;
using GameLogic.Othello.Ai;

namespace GameComponents
{
    public class CpuPlayer : IPlayer
    {
        private readonly OthelloGame game;
        private readonly Types.Color color;
        private readonly int level;

        public CpuPlayer(OthelloGame game, Types.Color color, int level)
        {
            this.game = game;
            this.color = color;
            this.level = level;

            game.Subscribe(PlayerUpEvent);
            game.PlayerReady(Types.Player.Of(color));
        }

        public string Description => "(cpu)";

        public bool IsInteractive => false;

        public bool CanResetGame => false;

        public bool IsLocal => true;

        public void Dispose()
        {
            game.Unsubscribe(PlayerUpEvent);
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

        async Task PlayerUpEvent(Types.PlayerStep step)
        {
            var delay = level == 0
                ? 1000
                : 0;
            if (step.Player.Color == color)
            {
                _ = new Timer(async (object? _) =>
                {
                    var play = level > 0
                        ? LookAheadStrategy.Execute(level + 1, game.State)
                        : RandomStrategy.Execute(Microsoft.FSharp.Core.FSharpOption<int>.None, game.State);
                    if (play is not null)
                    {
                        await MakePlay(play.Value.X, play.Value.Y);
                    }
                    else
                    {
                        await Pass();
                    }
                },
                null,
                delay,
                Timeout.Infinite);
            }
            await Task.CompletedTask;
        }
    }
}
