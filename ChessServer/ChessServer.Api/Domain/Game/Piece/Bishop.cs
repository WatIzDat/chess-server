namespace ChessServer.Api.Domain.Game.Piece;

public class Bishop(PlayerColor color) : Piece(color)
{
    public override List<Square> GetLegalSquares(Square fromSquare, Board board)
    {
        List<Square> legalSquares = [];
        
        AddSlidingPieceAttackSquares(ref legalSquares,
            1,
            7 - fromSquare.File,
            i => fromSquare.Up(i)?.Right(i),
            board);
        
        AddSlidingPieceAttackSquares(ref legalSquares,
            1,
            fromSquare.File,
            i => fromSquare.Up(i)?.Left(i),
            board);
        
        AddSlidingPieceAttackSquares(ref legalSquares,
            1,
            7 - fromSquare.File,
            i => fromSquare.Down(i)?.Right(i),
            board);
        
        AddSlidingPieceAttackSquares(ref legalSquares,
            1,
            fromSquare.File,
            i => fromSquare.Down(i)?.Left(i),
            board);
        
        return legalSquares;
    }
}
