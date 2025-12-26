using ChessServer.Api.Domain.Game.Piece;

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
        Piece.Piece piece = Pieces[move.FromSquare];
        
        List<Square> legalSquares = piece.GetLegalSquares(move.FromSquare, this);

        if (!legalSquares.Contains(move.ToSquare))
        {
            return false;
        }

        Square kingSquare = Pieces.First(kvp => kvp.Value is King && kvp.Value.Color == piece.Color).Key;

        if (IsKingInCheckByPiece(kingSquare, new Knight(piece.Color)) ||
            IsKingInCheckByPiece(kingSquare, new Bishop(piece.Color)) ||
            IsKingInCheckByPiece(kingSquare, new Rook(piece.Color)) ||
            IsKingInCheckByPiece(kingSquare, new Queen(piece.Color)) ||
            IsKingInCheckByPiece(kingSquare, new King(piece.Color)) ||
            IsKingInCheckByPiece(kingSquare, new Pawn(piece.Color.Opposite())))
        {
            return false;
        }

        return true;
    }

    private bool IsKingInCheckByPiece(Square testSquare, Piece.Piece testPiece)
    {
        List<Square> checkSquares = testPiece.GetLegalSquares(testSquare, this);

        return checkSquares.Any(square => Pieces[square].GetType() == testPiece.GetType() && Pieces[square].Color != testPiece.Color);
    }
}