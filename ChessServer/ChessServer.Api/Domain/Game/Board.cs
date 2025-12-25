namespace ChessServer.Api.Domain.Game;

public class Board
{
    public Dictionary<Square, Piece.Piece> Pieces { get; } = new();
    
    public PlayerColor PlayerToMove { get; set; }

    public Dictionary<PlayerColor, CastlingRights> CastlingRights { get; } = new()
    {
        { PlayerColor.White, new CastlingRights() },
        { PlayerColor.Black, new CastlingRights() }
    };

    public Dictionary<PlayerColor, Square?> EnPassantTargetSquares { get; } = new()
    {
        { PlayerColor.White, null },
        { PlayerColor.Black, null }
    };
    
    public int HalfmoveClock { get; set; }

    public Board()
    {
    }

    public bool CanOccupySquare(Square square)
    {
        return !Pieces.ContainsKey(square);
    }

    public bool CanOccupySquareWithCaptures(Square square, PlayerColor color)
    {
        return CanOccupySquare(square) || CanCaptureAtSquare(square, color);
    }

    public bool CanCaptureAtSquare(Square square, PlayerColor color)
    {
        return !CanOccupySquare(square) && Pieces[square].Color != color;
    }

    public bool IsMoveLegal(Move move)
    {
        //List<Square> legalMoves = Pieces[move.FromSquare].GetLegalMoves(TODO);

        //return legalMoves.Contains(move.ToSquare);

        return true;
    }
}