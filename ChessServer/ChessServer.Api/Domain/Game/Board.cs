using ChessServer.Api.Domain.Game.Piece;

namespace ChessServer.Api.Domain.Game;

public class Board
{
    public Dictionary<Square, Piece.Piece> Pieces { get; private set; } = new();
    
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

    public string GetPositionKey()
    {
        string[] fen = Fen.CreateFenFromBoard(this).Split(' ');

        string positionKey = string.Join(' ', fen[..3]);

        string epTargetSquare = fen[3];

        if (epTargetSquare != "-")
        {
            int epTargetFile = Square.CharFileToInt(epTargetSquare[0]);
            int epTargetRank = (int)char.GetNumericValue(epTargetSquare[1]);

            Square square = new(epTargetFile, epTargetRank, true);

            PlayerColor epTargetColor = square.Rank == 2 ? PlayerColor.White : PlayerColor.Black;
            
            Square upSquare = square.Up(perspective: epTargetColor)
                ?? throw new Exception("upSquare was somehow null");

            Square? leftSquare = upSquare.Left();
            Square? rightSquare = upSquare.Right();

            if ((leftSquare != null &&
                 Pieces.TryGetValue(leftSquare, out Piece.Piece? leftPiece) &&
                 leftPiece is Pawn leftPawn &&
                 leftPawn.Color == epTargetColor.Opposite()) ||
                (rightSquare != null &&
                 Pieces.TryGetValue(rightSquare, out Piece.Piece? rightPiece) &&
                 rightPiece is Pawn rightPawn &&
                 rightPawn.Color == epTargetColor.Opposite()))
            {
                positionKey += " " + epTargetSquare;
            }
        }

        return positionKey;
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

    public bool IsMoveLegal(Move move, PlayerColor color)
    {
        if (!Pieces.TryGetValue(move.FromSquare, out Piece.Piece? piece) ||
            PlayerToMove != color)
            return false;

        if (piece.Color != color)
            return false;
        
        List<Square> legalSquares = piece.GetLegalSquares(move.FromSquare, this);

        if (!legalSquares.Contains(move.ToSquare))
        {
            return false;
        }

        // Dictionary<Square, Piece.Piece> currentPiecesCopy = Pieces.ToDictionary();
        //
        // piece.MakeMove(Pieces, move);
        //
        // Square kingSquare = Pieces.First(kvp => kvp.Value is King && kvp.Value.Color == piece.Color).Key;

        if (IsKingInCheckAfterMove(move))
        {
            // Pieces = currentPiecesCopy;
            
            return false;
        }
        
        // Pieces = currentPiecesCopy;

        return true;
    }

    public GameResult MakeMove(Move move)
    {
        PlayerColor oppositeColor = Pieces[move.FromSquare].Color.Opposite();
        
        Pieces[move.FromSquare].MakeMove(this, move);
        
        if (Pieces.Count == 2 || 
            (Pieces.Count == 3 && (Pieces.Any(kvp => kvp.Value is Knight) || Pieces.Any(kvp => kvp.Value is Bishop))))
            return GameResult.DrawByInsufficientMaterial;

        var piecesCopy = Pieces.ToDictionary();
        
        foreach ((Square square, Piece.Piece piece) in piecesCopy)
        {
            if (piece.Color != oppositeColor)
                continue;
            
            List<Square> legalSquares = piece.GetLegalSquares(square, this);

            foreach (Square legalSquare in legalSquares)
            {
                if (!IsKingInCheckAfterMove(new Move(square, legalSquare)))
                {
                    return GameResult.None;
                }
            }
        }
        
        Square kingSquare = Pieces.First(kvp => kvp.Value is King && kvp.Value.Color == oppositeColor).Key;

        return IsKingInCheck(kingSquare, oppositeColor) ? GameResult.Checkmate : GameResult.Stalemate;
    }

    public bool IsKingInCheck(Square kingSquare, PlayerColor kingColor)
    {
        return IsKingInCheckByPiece(kingSquare, new Knight(kingColor)) ||
               IsKingInCheckByPiece(kingSquare, new Bishop(kingColor)) ||
               IsKingInCheckByPiece(kingSquare, new Rook(kingColor)) ||
               IsKingInCheckByPiece(kingSquare, new Queen(kingColor)) ||
               IsKingInCheckByPiece(kingSquare, new King(kingColor)) ||
               IsKingInCheckByPiece(kingSquare, new Pawn(kingColor));
    }

    public bool IsKingInCheckAfterMove(Move move)
    {
        Dictionary<Square, Piece.Piece> currentPiecesCopy = Pieces.ToDictionary();
        
        Piece.Piece piece = Pieces[move.FromSquare];
        
        piece.MakeMove(Pieces, move);
        
        Square kingSquare = Pieces.First(kvp => kvp.Value is King && kvp.Value.Color == piece.Color).Key;
        
        bool isInCheck = IsKingInCheckByPiece(kingSquare, new Knight(piece.Color)) ||
                         IsKingInCheckByPiece(kingSquare, new Bishop(piece.Color)) ||
                         IsKingInCheckByPiece(kingSquare, new Rook(piece.Color)) ||
                         IsKingInCheckByPiece(kingSquare, new Queen(piece.Color)) ||
                         IsKingInCheckByPiece(kingSquare, new King(piece.Color)) ||
                         IsKingInCheckByPiece(kingSquare, new Pawn(piece.Color));
        
        Pieces = currentPiecesCopy;

        return isInCheck;
    }

    private bool IsKingInCheckByPiece(Square kingSquare, Piece.Piece testPiece)
    {
        List<Square> checkSquares = testPiece is King king
            ? king.GetLegalSquaresNoCastling(kingSquare,
                this)
            : testPiece.GetLegalSquares(kingSquare,
                this);

        return checkSquares.Any(
            square => Pieces.ContainsKey(square) &&
                      Pieces[square].GetType() == testPiece.GetType() &&
                      Pieces[square].Color != testPiece.Color);
    }
}