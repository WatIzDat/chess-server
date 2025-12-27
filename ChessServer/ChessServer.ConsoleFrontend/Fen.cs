namespace ChessServer.ConsoleFrontend;

public static class Fen
{
    public static string FenToDisplayBoard(string fen)
    {
        List<char> board = [];

        foreach (char character in fen.Split(' ')[0])
        {
            if (character == '/')
                continue;
            
            if (char.IsDigit(character))
            {
                int emptyCount = (int)char.GetNumericValue(character);
                
                if (emptyCount is < 1 or > 8)
                    throw new ArgumentException("FEN invalid", nameof(fen));

                for (int i = 0; i < emptyCount; i++)
                {
                    board.Add(' ');
                }
            }
            else
            {
                board.Add(character switch
                {
                    'p' => '♟',
                    'n' => '♞',
                    'b' => '♝',
                    'r' => '♜',
                    'q' => '♛',
                    'k' => '♚',
                    'P' => '♙',
                    'N' => '♘',
                    'B' => '♗',
                    'R' => '♖',
                    'Q' => '♕',
                    'K' => '♔',
                    _ => throw new ArgumentException("FEN invalid", nameof(fen))
                });
            }
        }
        
        Console.WriteLine(board.Count);

        List<char> formattedBoard =
            """
            -------------------------------------------------
            |  x  |  x  |  x  |  x  |  x  |  x  |  x  |  x  |
            -------------------------------------------------
            |  x  |  x  |  x  |  x  |  x  |  x  |  x  |  x  |
            -------------------------------------------------
            |  x  |  x  |  x  |  x  |  x  |  x  |  x  |  x  |
            -------------------------------------------------
            |  x  |  x  |  x  |  x  |  x  |  x  |  x  |  x  |
            -------------------------------------------------
            |  x  |  x  |  x  |  x  |  x  |  x  |  x  |  x  |
            -------------------------------------------------
            |  x  |  x  |  x  |  x  |  x  |  x  |  x  |  x  |
            -------------------------------------------------
            |  x  |  x  |  x  |  x  |  x  |  x  |  x  |  x  |
            -------------------------------------------------
            |  x  |  x  |  x  |  x  |  x  |  x  |  x  |  x  |
            -------------------------------------------------
            """.ToList();

        int count = 0;

        for (var i = 0; i < formattedBoard.Count; i++)
        {
            if (formattedBoard[i] != 'x') continue;
            
            formattedBoard[i] = board[count];

            count++;
        }

        return new string(formattedBoard.ToArray());
    }
}