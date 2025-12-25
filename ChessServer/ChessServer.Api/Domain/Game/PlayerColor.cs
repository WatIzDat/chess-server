namespace ChessServer.Api.Domain.Game;

public enum PlayerColor
{
    White,
    Black
}

public static class PlayerColorExtensions
{
    public static PlayerColor Opposite(this PlayerColor color)
    {
        return color == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
    }
}