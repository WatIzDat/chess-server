namespace ChessServer.Api.Domain.Game.Piece;

public class Pawn(PlayerColor color) : Piece(color)
{
    public override List<Square> GetLegalSquares(Square fromSquare, Board board)
    {
        List<Square> legalSquares = [];
        
        Square upSquare = fromSquare.Up() ??
                          throw new ArgumentException("Pawn cannot be on 8th rank", nameof(fromSquare));

        if (!board.Pieces.ContainsKey(upSquare))
        {
            legalSquares.Add(upSquare);
        }

        if ((upSquare.Rank == 1 && Color == PlayerColor.White) || (upSquare.Rank == 6 && Color == PlayerColor.Black))
        {
            Square doubleStepSquare = upSquare.Up() ?? throw new Exception("Double step square somehow doesn't exist");

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