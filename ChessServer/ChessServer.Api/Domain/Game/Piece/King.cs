namespace ChessServer.Api.Domain.Game.Piece;

public class King(PlayerColor color) : Piece(color)
{
    public override List<Square> GetLegalMoves(Square fromSquare)
    {
        throw new NotImplementedException();
    }
}
