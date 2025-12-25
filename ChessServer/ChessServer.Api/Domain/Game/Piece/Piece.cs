namespace ChessServer.Api.Domain.Game.Piece;

public abstract class Piece(PlayerColor color)
{
   public PlayerColor Color { get; } = color;

   public abstract List<Square> GetLegalSquares(Square fromSquare, Board board);
}