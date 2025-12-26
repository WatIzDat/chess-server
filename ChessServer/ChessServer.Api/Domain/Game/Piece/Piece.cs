namespace ChessServer.Api.Domain.Game.Piece;

public abstract class Piece(PlayerColor color)
{
   public PlayerColor Color { get; } = color;

   public abstract List<Square> GetLegalSquares(Square fromSquare, Board board);

   public virtual void MakeMove(Dictionary<Square, Piece> pieces, Move move)
   {
      if (pieces[move.FromSquare].GetType() != GetType())
      {
         throw new ArgumentException("Incorrect piece", nameof(move));
      }
      
      pieces[move.ToSquare] = pieces[move.FromSquare];
         
      pieces.Remove(move.FromSquare);
   }

   public virtual void MakeMove(Board board, Move move)
   {
      if (!board.CanOccupySquare(move.ToSquare) || board.Pieces[move.FromSquare] is Pawn)
         board.HalfmoveClock = 0;
      else
         board.HalfmoveClock++;

      MakeMove(board.Pieces, move);
      
      board.PlayerToMove = board.PlayerToMove.Opposite();
   }

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