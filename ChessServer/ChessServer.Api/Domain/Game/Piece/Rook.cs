namespace ChessServer.Api.Domain.Game.Piece;

public class Rook(PlayerColor color) : Piece(color)
{
    public override List<Square> GetLegalSquares(Square fromSquare, Board board)
    {
        List<Square> legalSquares = [];
        
        AddSlidingPieceAttackSquares(ref legalSquares,
            fromSquare.File + 1,
            7,
            i => new Square(i,
                fromSquare.Rank),
            board);
        
        AddSlidingPieceAttackSquares(ref legalSquares,
            fromSquare.File - 1,
            0,
            i => new Square(i,
                fromSquare.Rank),
            board);
        
        AddSlidingPieceAttackSquares(ref legalSquares,
            fromSquare.Rank + 1,
            7,
            i => new Square(fromSquare.File,
                i),
            board);
        
        AddSlidingPieceAttackSquares(ref legalSquares,
            fromSquare.Rank - 1,
            0,
            i => new Square(fromSquare.File,
                i),
            board);
        
        return legalSquares;
    }

    private void AddSlidingPieceAttackSquares(ref List<Square> squares, int from, int to, Func<int, Square> squareProvider, Board board)
    {
        if (to == 0)
        {
            for (int i = from; i >= to; i--)
            {
                if (!CreateAndAddSquare(ref squares, i))
                    break;
            }
        }
        else
        {
            for (int i = from; i <= to; i++)
            {
                if (!CreateAndAddSquare(ref squares, i))
                    break;
            }
        }

        return;

        bool CreateAndAddSquare(ref List<Square> squares, int i)
        {
            Square square = squareProvider(i);

            if (board.CanOccupySquareWithCaptures(square, Color))
                squares.Add(square);

            return board.CanOccupySquare(square);
        }
    }
}
