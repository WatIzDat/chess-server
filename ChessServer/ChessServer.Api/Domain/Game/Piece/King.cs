namespace ChessServer.Api.Domain.Game.Piece;

public class King(PlayerColor color) : Piece(color)
{
    public override List<Square> GetLegalSquares(Square fromSquare, Board board)
    {
        List<Square?> legalSquares = 
        [
            fromSquare.Up(),
            fromSquare.Right(),
            fromSquare.Left(),
            fromSquare.Down(),
            fromSquare.Up()?.Right(),
            fromSquare.Up()?.Left(),
            fromSquare.Down()?.Right(),
            fromSquare.Down()?.Left(),
        ];
        
        return legalSquares.Where(square => square != null && board.CanOccupySquareWithCaptures(square, Color)).ToList()!;
    }
}
