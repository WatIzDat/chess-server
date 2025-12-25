namespace ChessServer.Api.Domain.Game.Piece;

public class Pawn(PlayerColor color) : Piece(color)
{
    public override List<Square> GetLegalSquares(Square fromSquare, Board board)
    {
        List<Square> legalSquares = [];

        Square upSquare = fromSquare.Up(perspective: Color) ?? throw new ArgumentException("Square invalid", nameof(fromSquare));

        if (!board.Pieces.ContainsKey(upSquare))
        {
            legalSquares.Add(upSquare);
        }

        if ((fromSquare.Rank == 1 && Color == PlayerColor.White) || (fromSquare.Rank == 6 && Color == PlayerColor.Black))
        {
            Square doubleStepSquare = upSquare.Up(perspective: Color) ?? throw new Exception("Double step square somehow doesn't exist");

            if (!board.Pieces.ContainsKey(doubleStepSquare))
            {
                legalSquares.Add(doubleStepSquare);
            }
        }
        
        Square? leftCapture = upSquare.Left();
        Square? rightCapture = upSquare.Right();

        if (leftCapture != null && (board.Pieces.ContainsKey(leftCapture) || board.EnPassantTargetSquares[Color.Opposite()] == leftCapture))
        {
            legalSquares.Add(leftCapture);
        }
        if (rightCapture != null && (board.Pieces.ContainsKey(rightCapture) || board.EnPassantTargetSquares[Color.Opposite()] == rightCapture))
        {
            legalSquares.Add(rightCapture);
        }
        
        return legalSquares;
    }
}