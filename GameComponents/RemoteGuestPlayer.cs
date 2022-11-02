using GameLogic.Othello;

namespace GameComponents
{
    public class RemoteGuestPlayer : IPlayer
    {
        private readonly OthelloGame game;
        private readonly Types.Color guestColor;
        private readonly IHostConnection connection = default!;
        Timer? connectionTimer;

        public RemoteGuestPlayer(OthelloGame game, string gameId, Types.Color color, IHostGuestBridge bridge)
        {
            this.game = game;
            this.guestColor = color;

            game.Subscribe(PlayMade);

            connection = bridge.CreateConnectionToHost(gameId, color.AsPlayerColor(), MakePlay, Pass, HostNewGame);

            var createEvent = new AutoResetEvent(false);
            connectionTimer = new Timer(async (object? stateInfo) =>
            {
                if (await connection.ConnectToHost())
                {
                    createEvent.WaitOne();
                    connectionTimer!.Dispose();
                    connectionTimer = null;
                    this.game.PlayerReady(Types.Player.Of(this.guestColor));
                }
            },
            null,
            0,
            1000);
            createEvent.Set();

        }

        public string Description => "(remote host)";

        public bool IsInteractive => false;

        public bool IsLocal => false;

        public bool CanResetGame => false;

        public void Dispose()
        {
            connection.Dispose();
            connectionTimer?.Dispose();
            game.Unsubscribe(PlayMade);
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


        public async Task HostNewGame()
        {
            await game.GameResetByHost();
            await connection.GuestReady();
        }

        async Task PlayMade(Types.Color color, Types.Square? play)
        {
            if (guestColor == color.Next)
            {
                if (play is not null)
                {
                    await connection.GuestPlayMade(play.X, play.Y);
                }
                else
                {
                    await connection.GuestPassMade();
                }
            }
        }
    }
}
