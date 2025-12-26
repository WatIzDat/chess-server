namespace ChessServer.Api.Domain.Game.Piece;

public class Queen(PlayerColor color) : Piece(color)
{
    public override List<Square> GetLegalSquares(Square fromSquare, Board board)
    {
        Rook rook = new(Color);
        Bishop bishop = new(Color);
        
        List<Square> legalSquares = rook.GetLegalSquares(fromSquare, board);
        legalSquares.AddRange(bishop.GetLegalSquares(fromSquare, board));
        
        return legalSquares;
    }
}
