namespace ChessServer.Api.Domain.Game.Piece;

public class Knight(PlayerColor color) : Piece(color)
{
    public override List<Square> GetLegalSquares(Square fromSquare, Board board)
    {
        List<Square?> legalSquares = 
        [
            fromSquare.Up(2)?.Right(),
            fromSquare.Up(2)?.Left(),
            fromSquare.Down(2)?.Right(),
            fromSquare.Down(2)?.Left(),
            fromSquare.Right(2)?.Up(),
            fromSquare.Right(2)?.Down(),
            fromSquare.Left(2)?.Up(),
            fromSquare.Left(2)?.Down(),
        ];
        
        return legalSquares.Where(square => square != null && board.CanOccupySquareWithCaptures(square, Color)).ToList()!;
    }
}