using ChessServer.Api.Domain.Game.Piece;

namespace ChessServer.Api.Domain.Game;

public static class Fen
{
    public const string InitialPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    
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

    private static char GetCharOfPiece(Piece.Piece piece)
    {
        char character = piece switch
        {
            Pawn => 'p',
            Knight => 'n',
            Bishop => 'b',
            Rook => 'r',
            Queen => 'q',
            King => 'k',
            _ => throw new ArgumentException("Piece was not valid", nameof(piece))
        };
        
        return piece.Color == PlayerColor.White ? char.ToUpperInvariant(character) : character;
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
                    default:
                        throw new ArgumentException("Invalid castling rights", nameof(fen));
                }
            }
        }

        if (epTargetSquare != "-")
        {
            int epTargetFile = Square.CharFileToInt(epTargetSquare[0]);
            int epTargetRank;

            switch (epTargetSquare[1])
            {
                case '3':
                    epTargetRank = 2;
                
                    board.EnPassantTargetSquares[PlayerColor.White] = new Square(epTargetFile, epTargetRank);
                    break;
                case '6':
                    epTargetRank = 5;
                
                    board.EnPassantTargetSquares[PlayerColor.Black] = new Square(epTargetFile, epTargetRank);
                    break;
                default:
                    throw new ArgumentException("Invalid en passant target rank", nameof(fen));
            }
        }

        if (halfmoveClock != "0")
        {
            board.HalfmoveClock = int.Parse(halfmoveClock);
        }

        return board;
    }

    public static string CreateFenFromBoard(Board board)
    {
        string fen = "";

        var pieces = board.Pieces.OrderBy(kvp => kvp.Key, new SquareFenComparer());

        int file = -1;
        int rank = 7;

        foreach ((Square square, Piece.Piece piece) in pieces)
        {
            //Console.WriteLine($"{square.File} {square.Rank}");
            if (square.Rank != rank)
            {
                if (file < 7 || rank - square.Rank > 1)
                {
                    if (file < 7)
                    {
                        fen += $"{7 - file}";
                    }

                    fen += "/";
                    
                    for (int i = 0; i < rank - square.Rank - 1; i++)
                    {
                        fen += "8/";
                    }
                    
                    if (square.File > 0)
                    {
                        fen += square.File;
                    }
                    
                    file = square.File;
                }
                else
                {
                    fen += "/";
                    
                    if (square.File > 0)
                    {
                        fen += square.File;
                    }
                }
            }
            
            rank = square.Rank;
            
            if (square.File - file > 1)
            {
                fen += square.File - file - 1;
            }
            
            file = square.File;

            fen += GetCharOfPiece(piece);
        }

        if (file < 7)
        {
            fen += 7 - file;
        }

        if (rank > 0)
        {
            for (int i = 0; i < rank; i++)
            {
                fen += "/8";
            }
        }
        
        fen += board.PlayerToMove == PlayerColor.White ? " w " : " b ";

        if (!board.CastlingRights[PlayerColor.White].Kingside &&
            !board.CastlingRights[PlayerColor.White].Queenside &&
            !board.CastlingRights[PlayerColor.Black].Kingside &&
            !board.CastlingRights[PlayerColor.Black].Queenside)
        {
            fen += "-";
        }
        else
        {
            if (board.CastlingRights[PlayerColor.White].Kingside) fen += "K";
            if (board.CastlingRights[PlayerColor.White].Queenside) fen += "Q";
            if (board.CastlingRights[PlayerColor.Black].Kingside) fen += "k";
            if (board.CastlingRights[PlayerColor.Black].Queenside) fen += "q";
        }

        fen += " ";
        
        if (board.EnPassantTargetSquares[PlayerColor.White] != null)
            fen += board.EnPassantTargetSquares[PlayerColor.White]!.ToString();
        else if (board.EnPassantTargetSquares[PlayerColor.Black] != null)
            fen += board.EnPassantTargetSquares[PlayerColor.Black]!.ToString();
        else
            fen += "-";

        fen += " " + board.HalfmoveClock;

        // TODO: Make this the actual move count (just doing 1 right now to make it a valid FEN)
        fen += " 1";

        return fen;
    }

    private class SquareFenComparer : IComparer<Square>
    {
        public int Compare(Square? x, Square? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (y is null) return 1;
            if (x is null) return -1;
            var rankComparison = y.Rank.CompareTo(x.Rank);
            if (rankComparison != 0) return rankComparison;
            return x.File.CompareTo(y.File);
        }
    }
}
