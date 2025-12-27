namespace ChessServer.Api.Domain.Game.Piece;

public class King(PlayerColor color) : Piece(color)
{
    public override List<Square> GetLegalSquares(Square fromSquare, Board board)
    {
        List<Square> legalSquares = GetLegalSquaresNoCastling(fromSquare, board);

        if (board.CastlingRights[Color].Kingside &&
            board.CanOccupySquare(fromSquare.Right() ?? throw new Exception("Castling rights invalid")) &&
            board.CanOccupySquare(fromSquare.Right(2) ?? throw new Exception("Castling rights invalid")) &&
            !board.IsKingInCheck(fromSquare, Color) &&
            !board.IsKingInCheckAfterMove(new Move(fromSquare, fromSquare.Right()!, _ => null)))
        {
            legalSquares.Add(fromSquare.Right(2)!);
        }
        
        if (board.CastlingRights[Color].Queenside &&
            board.CanOccupySquare(fromSquare.Left() ?? throw new Exception("Castling rights invalid")) &&
            board.CanOccupySquare(fromSquare.Left(2) ?? throw new Exception("Castling rights invalid")) &&
            board.CanOccupySquare(fromSquare.Left(3) ?? throw new Exception("Castling rights invalid")) &&
            !board.IsKingInCheck(fromSquare, Color) &&
            !board.IsKingInCheckAfterMove(new Move(fromSquare, fromSquare.Left()!, _ => null)))
        {
            legalSquares.Add(fromSquare.Left(2)!);
        }

        return legalSquares;
    }

    public List<Square> GetLegalSquaresNoCastling(Square fromSquare, Board board)
    {
        List<Square?> legalSquares = 
        [
            fromSquare.Up(),
            fromSquare.Right(),
            fromSquare.Left(),
            fromSquare.Down(),
            fromSquare.Up()?.Right(),
            fromSquare.Up()?.Left(),
            fromSquare.Down()?.Right(),
            fromSquare.Down()?.Left(),
        ];
        
        return legalSquares.Where(square => square != null && board.CanOccupySquareWithCaptures(square, Color)).ToList()!;
    }

    public override void MakeMove(Board board, Move move)
    {
        base.MakeMove(board, move);

        board.CastlingRights[Color].Kingside = false;
        board.CastlingRights[Color].Queenside = false;
    }
}
