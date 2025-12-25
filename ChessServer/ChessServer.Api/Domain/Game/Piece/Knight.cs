namespace ChessServer.Api.Domain.Game.Piece;

public class Knight(PlayerColor color) : Piece(color)
{
    public override List<Square> GetLegalSquares(Square fromSquare, Board board)
    {
        throw new NotImplementedException();
    }
}