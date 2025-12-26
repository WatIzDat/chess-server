using ChessServer.Api.Domain.Game.Piece;

namespace ChessServer.Api.Domain.Game;

public class Move(Square fromSquare, Square toSquare, Func<PlayerColor, Piece.Piece?> getPromotion)
{
    public Square FromSquare { get; } = fromSquare;
    public Square ToSquare { get; } = toSquare;
    
    public Func<PlayerColor, Piece.Piece?> GetPromotion { get; } = getPromotion;

    public static Move Create(string move)
    {
        if (move.Length != 4 && move.Length != 5)
        {
            throw new ArgumentException("Move must be 4 or 5 characters long");
        }

        Square fromSquare = new(move[0], int.Parse(move[1].ToString()), true);
        Square toSquare = new(move[2], int.Parse(move[3].ToString()), true);
        
        return new Move(fromSquare, toSquare, color => CreatePromotionPieceFromChar(move[4], color));
    }

    public string Serialize()
    {
        return FromSquare.ToString() + ToSquare;
    }

    private static Piece.Piece? CreatePromotionPieceFromChar(char piece, PlayerColor color)
    {
        return char.ToLower(piece) switch
        {
            'n' => new Knight(color),
            'b' => new Bishop(color),
            'r' => new Rook(color),
            'q' => new Queen(color),
            _ => null
        };
    }
}