using GameLogic.Othello;
using Microsoft.AspNetCore.Components;
using Microsoft.FSharp.Collections;

namespace GameComponents
{
    public partial class OthelloGamePanel : IDisposable
    {
        [Inject]
        public OthelloService OthelloService { get; set; } = default!;

        OthelloGame game = default!;
        Dictionary<Types.Color, IPlayer> players = new();

        readonly FSharpList<int> rangeY = Board.YRange;
        readonly FSharpList<char> rangeX = Board.XRange;

        bool CanResetGame =>
            players.Count > 0 &&
            (players[Types.Color.Black].CanResetGame || players[Types.Color.Black].IsLocal) &&
            (players[Types.Color.White].CanResetGame || players[Types.Color.White].IsLocal);

        protected override async Task OnInitializedAsync()
        {
            await NewGame();
        }

        async Task NewGame()
        {
            game?.Unsubscribe(GameStateChange);

            (game, players) = OthelloService.CreateGame();

            game.Subscribe(GameStateChange);
            await GameStateChange();
        }

        async Task GameStateChange()
        {
            if (!game.IsGameOver && game.PlayersAreReady)
            {
                await game.PlayerUp();
            }
            await InvokeAsync(StateHasChanged);
        }

        async Task MakePlay(char x, int y)
        {
            if (IsPlayerInteractive && game.HasPotentialPlay(x, y))
            {
                await players[game.GetPlayerUpColor()!].MakePlay(x, y);
            }
        }

        async Task Pass()
        {
            if (IsPlayerInteractive && !game.HasPotentialPlays)
            {
                await players[game.GetPlayerUpColor()!].Pass();
            }
        }

        static string ColorText(Types.Color color) => color == Types.Color.Black ? "Black" : "White";

        public void Dispose()
        {
            game?.Unsubscribe(GameStateChange);

            foreach (var player in players)
            {
                player.Value.Dispose();
            }
        }

        string PlayerLabel => game.GetPlayerUpColor() is not null ? "Player Up" : "Game Over";
        string PlayerValue
        {
            get
            {
                var playerUpColor = game.GetPlayerUpColor();
                var winnerColor = game.GetWinnerColor();
                return playerUpColor is not null
                    ? $"{ColorText(playerUpColor)} {players[playerUpColor].Description}"
                    : winnerColor is not null
                        ? $"{ColorText(winnerColor)} is the winner!"
                        : "It's a tie.";
            }
        }
                
        int BlackCount => game?.ColorCount(Types.Color.Black) ?? 0;
        int WhiteCount => game?.ColorCount(Types.Color.White) ?? 0;
        bool MustPass => !(game?.IsGameOver ?? true) && !(game?.HasPotentialPlays ?? false);
        bool IsPlayerInteractive => !(game?.IsGameOver ?? true) && players[game.GetPlayerUpColor()!].IsInteractive;
    }
}
