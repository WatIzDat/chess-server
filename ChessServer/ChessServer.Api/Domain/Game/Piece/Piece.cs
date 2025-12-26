namespace ChessServer.Api.Domain.Game.Piece;

public abstract class Piece(PlayerColor color)
{
   public PlayerColor Color { get; } = color;

   public abstract List<Square> GetLegalSquares(Square fromSquare, Board board);
   
   protected void AddSlidingPieceAttackSquares(ref List<Square> squares, int from, int to, Func<int, Square?> squareProvider, Board board, bool iterateReverse = false)
   {
      if (iterateReverse)
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
         Square? square = squareProvider(i);

         if (square == null)
            return false;

         if (board.CanOccupySquareWithCaptures(square, Color))
            squares.Add(square);

         return board.CanOccupySquare(square);
      }
   }
}