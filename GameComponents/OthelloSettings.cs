namespace GameComponents
{
    public class OthelloSettings
    {
        public enum PlayerType
        {
            Local,
            Cpu,
            RemoteHost,
            RemoteGuest
        }

        public PlayerType BlackPlayerType { get; set; }
        public PlayerType WhitePlayerType { get; set; }
        public int BlackCpuLevel { get; set; }
        public int WhiteCpuLevel { get; set; }
        public string? RemoteGameId { get; set; }
    }
}
