namespace ChessServer.Api.Domain.Game.Piece;

public class King(PlayerColor color) : Piece(color)
{
    public override List<Square> GetLegalSquares(Square fromSquare, Board board)
    {
        throw new NotImplementedException();
    }
}
