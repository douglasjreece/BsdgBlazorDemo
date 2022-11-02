using Microsoft.AspNetCore.Components;

namespace GameComponents
{
    public partial class OthelloSettingsPanel
    {
        enum PlayerColor { Black, White }

        [Inject]
        public OthelloSettings Settings { get; set; } = default!;

        void SetPlayerType(PlayerColor color, OthelloSettings.PlayerType type)
        {
            switch (color)
            {
                case PlayerColor.Black: Settings.BlackPlayerType = type; break;
                case PlayerColor.White: Settings.WhitePlayerType = type; break;
            }
        }

        void SetCpuLevel(PlayerColor color, int level)
        {
            switch (color)
            {
                case PlayerColor.Black: Settings.BlackCpuLevel = level; break;
                case PlayerColor.White: Settings.WhiteCpuLevel = level; break;
            }
        }

        void GameIdChange(ChangeEventArgs args)
        {
            Settings.RemoteGameId = (string?)args.Value;
        }
    }
}
