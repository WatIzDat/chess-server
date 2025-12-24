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

    public Board()
    {
    }

    public bool IsMoveLegal(Move move)
    {
        List<Square> legalMoves = Pieces[move.FromSquare].GetLegalMoves(TODO);

        return legalMoves.Contains(move.ToSquare);
    }
}