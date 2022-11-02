using GameLogic.Othello;

namespace GameComponents
{
    public static class TypesColorEx
    {
        public static PlayerColor AsPlayerColor(this Types.Color color)
        {
            return color == Types.Color.Black
                ? PlayerColor.Black
                : PlayerColor.White;
        }
    }
}
