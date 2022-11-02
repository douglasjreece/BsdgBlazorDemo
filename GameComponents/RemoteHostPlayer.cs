using GameLogic.Othello;

namespace GameComponents
{
    public class RemoteHostPlayer : IPlayer
    {
        private readonly OthelloGame game;
        private readonly Types.Color color;
        private readonly IGuestConnection connection = default!;
        Timer? connectionTimer;

        public RemoteHostPlayer(OthelloGame game, string gameId, Types.Color color, IHostGuestBridge bridge)
        {
            this.game = game;
            this.color = color;

            game.Subscribe(PlayMade);
            game.Subscribe(PlayerUp);

            connection = bridge.CreateConnectionToGuest(gameId, color.AsPlayerColor(), MakePlay, Pass);

            var createEvent = new AutoResetEvent(false);
            connectionTimer = new Timer(async (object? stateInfo) =>
            {
                if (await connection.ConnectToGuest())
                {
                    createEvent.WaitOne();
                    connectionTimer!.Dispose();
                    connectionTimer = null;
                    this.game.PlayerReady(Types.Player.Of(this.color));
                }
            },
            null,
            0,
            1000);
            createEvent.Set();
        }

        public string Description => "(remote guest)";

        public bool IsInteractive => false;

        public bool IsLocal => false;

        public bool CanResetGame => true;

        public void Dispose()
        {
            connection.Dispose();
            connectionTimer?.Dispose();
            game.Unsubscribe(PlayMade);
            game.Unsubscribe(PlayerUp);
        }

        public async Task MakePlay(char x, int y)
        {
            await game.MakePlay(Types.Square.At(x, y));
        }

        public Task MakePlay(Types.Position position)
        {
            throw new NotImplementedException();
        }

        public async Task Pass()
        {
            await game.MakePlay(null);
        }

        async Task PlayMade(Types.Color color, Types.Square? play)
        {
            if (color != this.color)
            {
                if (play is not null)
                {
                    await connection.HostPlayMade(play.X, play.Y);
                }
                else
                {
                    await connection.HostPassMade();
                }
            }
        }

        async Task PlayerUp(Types.PlayerStep step)
        {
            await Task.CompletedTask;
        }
    }
}
