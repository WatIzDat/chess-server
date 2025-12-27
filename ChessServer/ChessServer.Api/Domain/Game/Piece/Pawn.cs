namespace ChessServer.Api.Domain.Game.Piece;

public class Pawn(PlayerColor color) : Piece(color)
{
    public override List<Square> GetLegalSquares(Square fromSquare, Board board)
    {
        List<Square> legalSquares = [];

        Square upSquare = fromSquare.Up(perspective: Color) ?? throw new ArgumentException("Square invalid", nameof(fromSquare));

        if (board.CanOccupySquare(upSquare))
        {
            legalSquares.Add(upSquare);
        }

        if ((fromSquare.Rank == 1 && Color == PlayerColor.White) || (fromSquare.Rank == 6 && Color == PlayerColor.Black))
        {
            Square doubleStepSquare = upSquare.Up(perspective: Color) ?? throw new Exception("Double step square somehow doesn't exist");

            if (board.CanOccupySquare(doubleStepSquare))
            {
                legalSquares.Add(doubleStepSquare);
            }
        }
        
        Square? leftCapture = upSquare.Left();
        Square? rightCapture = upSquare.Right();
        
        Console.WriteLine($"En passant square: {board.EnPassantTargetSquares[Color.Opposite()]}");

        if (leftCapture != null && (board.CanCaptureAtSquare(leftCapture, Color) || board.EnPassantTargetSquares[Color.Opposite()] == leftCapture))
        {
            legalSquares.Add(leftCapture);
        }
        if (rightCapture != null && (board.CanCaptureAtSquare(rightCapture, Color) || board.EnPassantTargetSquares[Color.Opposite()] == rightCapture))
        {
            legalSquares.Add(rightCapture);
        }
        
        return legalSquares;
    }

    public override void MakeMove(Dictionary<Square, Piece> pieces, Move move)
    {
        Pawn pawn = (Pawn)pieces[move.FromSquare];
        
        base.MakeMove(pieces, move);

        if ((pawn.Color == PlayerColor.White && move.ToSquare.Rank == 7) ||
            (pawn.Color == PlayerColor.Black && move.ToSquare.Rank == 1))
        {
            Piece? promotion = move.GetPromotion(Color);
            
            if (promotion != null)
                pieces[move.ToSquare] = promotion;
        }
    }

    public override void MakeMove(Board board, Move move)
    {
        if (move.ToSquare.File != move.FromSquare.File && !board.CanCaptureAtSquare(move.ToSquare, Color))
        {
            board.Pieces.Remove(move.ToSquare.Down(perspective: Color) ?? throw new ArgumentException("En passant invalid", nameof(move)));
        }
        
        base.MakeMove(board, move);

        if ((Color == PlayerColor.White && move.ToSquare.Rank == move.FromSquare.Rank + 2) ||
            (Color == PlayerColor.Black && move.ToSquare.Rank == move.FromSquare.Rank - 2))
        {
            board.EnPassantTargetSquares[Color] = move.ToSquare.Down(perspective: Color);
        }
        else
        {
            board.EnPassantTargetSquares[Color] = null;
        }
    }
}