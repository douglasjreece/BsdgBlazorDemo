namespace GameComponents
{
    public enum PlayerColor
    {
        Black,
        White
    }

    public static class PlayerColorEx
    {
        public static PlayerColor Opposite(this PlayerColor color)
        {
            return color switch
            {
                PlayerColor.Black => PlayerColor.White,
                PlayerColor.White => PlayerColor.Black,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
