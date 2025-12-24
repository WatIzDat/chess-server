using ChessServer.Api.Domain.Game.Piece;

namespace ChessServer.Api.Domain.Game;

public static class Fen
{
    private static readonly char[] ValidPieces = ['p', 'n', 'b', 'r', 'q', 'k', 'P', 'N', 'B', 'R', 'Q', 'K'];
    
    private static Piece.Piece CreatePieceFromChar(char piece)
    {
        return piece switch
        {
            'p' => new Pawn(PlayerColor.Black),
            'n' => new Knight(PlayerColor.Black),
            'b' => new Bishop(PlayerColor.Black),
            'r' => new Rook(PlayerColor.Black),
            'q' => new Queen(PlayerColor.Black),
            'k' => new King(PlayerColor.Black),
            'P' => new Pawn(PlayerColor.White),
            'N' => new Knight(PlayerColor.White),
            'B' => new Bishop(PlayerColor.White),
            'R' => new Rook(PlayerColor.White),
            'Q' => new Queen(PlayerColor.White),
            'K' => new King(PlayerColor.White),
            _ => throw new ArgumentException("Piece was not valid", nameof(piece))
        };
    }
    
    public static Board CreateBoardFromFen(string fen)
    {
        Board board = new();
        
        string[] splitFen = fen.Split(' ');
        string pieces = splitFen[0];
        string sideToMove = splitFen[1];
        string castlingRights = splitFen[2];
        string epTargetSquare = splitFen[3];
        string halfmoveClock = splitFen[4];

        int file = 0;
        int rank = 7;

        foreach (char character in pieces)
        {
            if (character == '/')
            {
                rank--;
                file = 0;

                continue;
            }

            if (file is < 0 or > 7)
            {
                throw new ArgumentException("Invalid file", nameof(fen));
            }

            if (ValidPieces.Contains(character))
            {
                Square square = new(file, rank);
                
                board.Pieces[square] = CreatePieceFromChar(character);
            }
            else
            {
                if (!char.IsDigit(character))
                {
                    throw new ArgumentException("Character not digit", nameof(fen));
                }
                
                int emptyFiles = (int)char.GetNumericValue(character);
                
                if (emptyFiles is < 1 or > 8)
                {
                    throw new ArgumentException("Invalid file", nameof(fen));
                }
                
                file += emptyFiles - 1;
            }

            file++;
        }

        board.PlayerToMove = sideToMove switch
        {
            "w" => PlayerColor.White,
            "b" => PlayerColor.Black,
            _ => throw new ArgumentException("Invalid side to move", nameof(fen))
        };

        if (castlingRights != "-")
        {
            foreach (char character in castlingRights)
            {
                switch (character)
                {
                    case 'K':
                        board.CastlingRights[PlayerColor.White].Kingside = true;
                        break;
                    case 'Q':
                        board.CastlingRights[PlayerColor.White].Queenside = true;
                        break;
                    case 'k':
                        board.CastlingRights[PlayerColor.Black].Kingside = true;
                        break;
                    case 'q':
                        board.CastlingRights[PlayerColor.Black].Queenside = true;
                        break;
                }
            }
        }
    }
}