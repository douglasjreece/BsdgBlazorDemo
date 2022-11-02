using GameLogic.Othello;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

namespace GameComponents
{
    public class OthelloGame
    {
        readonly HashSet<Types.Color> playersReady = new();

        Types.Game game = GameModule.Initial;
        event Func<Task> OnStateChange = () => Task.CompletedTask;
        event Func<Types.PlayerStep, Task> OnPlayerUp = (_) => Task.CompletedTask;
        event Func<Types.Color, Types.Square?, Task> OnPlayMade = (_, __) => Task.CompletedTask;

        public Types.GameState State => game.State;
        public string? RemoteGameId { get; set; }
        public bool PlayersAreReady => playersReady.Count > 1;

        public void Subscribe(Func<Task> onStateChange)
        {
            this.OnStateChange += onStateChange;
        }

        public void Unsubscribe(Func<Task> onStateChange)
        {
            this.OnStateChange -= onStateChange;
        }

        public void Subscribe(Func<Types.Color, Types.Square?, Task> onPlayMade)
        {
            this.OnPlayMade += onPlayMade;
        }

        public void Unsubscribe(Func<Types.Color, Types.Square?, Task> onPlayMade)
        {
            this.OnPlayMade -= onPlayMade;
        }

        public void Subscribe(Func<Types.PlayerStep, Task> onPlayerUp)
        {
            this.OnPlayerUp += onPlayerUp;
        }

        public void Unsubscribe(Func<Types.PlayerStep, Task> onPlayerUp)
        {
            this.OnPlayerUp -= onPlayerUp;
        }

        public void PlayerReady(Types.Player player)
        {
            playersReady.Add(player.Color);
            OnStateChange.Invoke();
        }

        public Types.Color? GetPositionColor(char x, int y)
        {
            var position = PositionsModule.PositionAt(new Types.Square(x, y), State.Positions);
            return position is not null
                ? position.Value.Color
                : null;
        }

        public int ColorCount(Types.Color color)
        {
            return PositionsModule.ColorCount(color, State.Positions);
        }

        public Types.Color? GetPotentialPlayColor(char x, int y)
        {
            var playerUp = State.Step as Types.Step.PlayerUp;

            if (playerUp is not null)
            {
                var position = PositionsModule.PositionAt(new Types.Square(x, y), playerUp.Item.PotentialPlays);
                return position is not null
                    ? position.Value.Color
                    : null;
            }
            return null;
        }

        public bool HasPotentialPlays
        {
            get 
            {
                var playerUp = State.Step as Types.Step.PlayerUp;
                return playerUp is not null && playerUp.Item.PotentialPlays.Length > 0;
            }
        }

        public bool HasPotentialPlay(char x, int y)
        {
            var playerUp = State.Step as Types.Step.PlayerUp;

            if (playerUp is not null)
            {
                var position = PositionsModule.PositionAt(new Types.Square(x, y), playerUp.Item.PotentialPlays);
                return position is not null;
            }
            return false;
        }

        public FSharpList<Types.Position> PotentialPlays => State.Step.AsPlayerUp.PotentialPlays;

        public Types.Color? GetPlayerUpColor()
        {
            var playerUp = State.Step as Types.Step.PlayerUp;
            return playerUp is not null
                ? playerUp.Item.Player.Color
                : null;
        }

        public bool IsPlayerUp(Types.Color color)
        {
            return GetPlayerUpColor() == color;
        }

        public bool IsGameOver => State.Step is Types.Step.GameOver;

        public Types.Color? GetWinnerColor()
        {
            var gameOver = State.Step as Types.Step.GameOver;
            return gameOver is not null
                ? gameOver.Item.Winner?.Value.Color
                : null;
        }

        public async Task MakePlay(Types.Square? play)
        {
            var playerColor = GetPlayerUpColor()!;
            game = GameModule.NextStep(game, play is not null ? play : FSharpOption<Types.Square>.None);
            await OnPlayMade.Invoke(playerColor, play);
            await OnStateChange.Invoke();
        }

        public async Task PlayerUp()
        {
            var color = GetPlayerUpColor();
            await OnPlayerUp.Invoke(State.Step.AsPlayerUp);
        }

        public async Task GameResetByHost()
        {
            game = GameModule.Initial;
            await OnStateChange.Invoke();
        }
    }
}
