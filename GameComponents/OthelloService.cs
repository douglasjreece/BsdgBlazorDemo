using GameLogic.Othello;

namespace GameComponents
{
    public class OthelloService
    {
        private readonly OthelloSettings settings;
        private readonly IHostGuestBridge hostGuestBridge;

        public OthelloService(OthelloSettings settings, IHostGuestBridge hostGuestBridge)
        {
            this.settings = settings;
            this.hostGuestBridge = hostGuestBridge;
        }

        public (OthelloGame State, Dictionary<Types.Color, IPlayer> Players) CreateGame()
        {
            OthelloGame state = new()
            {
                RemoteGameId = settings.RemoteGameId
            };
            Dictionary<Types.Color, IPlayer> players = new()
            {
                { Types.Color.Black, CreatePlayer(Types.Color.Black, state) },
                { Types.Color.White, CreatePlayer(Types.Color.White, state) },
            };

            return (state, players);
        }

        IPlayer CreatePlayer(Types.Color color, OthelloGame state)
        {
            var playerType = color == Types.Color.Black
                ? settings.BlackPlayerType
                : settings.WhitePlayerType;

            return playerType switch
            {
                OthelloSettings.PlayerType.Local => new LocalPlayer(state, color),
                OthelloSettings.PlayerType.Cpu => new CpuPlayer(state, color, color.IsBlack ? settings.BlackCpuLevel : settings.WhiteCpuLevel),
                OthelloSettings.PlayerType.RemoteHost => new RemoteHostPlayer(state, settings.RemoteGameId!, color, hostGuestBridge),
                OthelloSettings.PlayerType.RemoteGuest => new RemoteGuestPlayer(state, settings.RemoteGameId!, color, hostGuestBridge),
                _ => throw new NotImplementedException()
            };
        }
    }
}
