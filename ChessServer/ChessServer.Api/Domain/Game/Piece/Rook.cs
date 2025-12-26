namespace ChessServer.Api.Domain.Game.Piece;

public class Rook(PlayerColor color) : Piece(color)
{
    public override List<Square> GetLegalSquares(Square fromSquare, Board board)
    {
        List<Square> legalSquares = [];
        
        AddSlidingPieceAttackSquares(legalSquares,
            fromSquare.File + 1,
            7,
            i => new Square(i,
                fromSquare.Rank),
            board);
        
        AddSlidingPieceAttackSquares(legalSquares,
            fromSquare.File - 1,
            0,
            i => new Square(i,
                fromSquare.Rank),
            board,
            true);
        
        AddSlidingPieceAttackSquares(legalSquares,
            fromSquare.Rank + 1,
            7,
            i => new Square(fromSquare.File,
                i),
            board);
        
        AddSlidingPieceAttackSquares(legalSquares,
            fromSquare.Rank - 1,
            0,
            i => new Square(fromSquare.File,
                i),
            board,
            true);
        
        return legalSquares;
    }

}
