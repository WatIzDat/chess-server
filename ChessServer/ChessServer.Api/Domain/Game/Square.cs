namespace ChessServer.Api.Domain.Game;

public record Square
{
    public int File { get; private set; }
    public int Rank { get; private set; }

    private static readonly char[] Files = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'];
    
    public Square(char file, int rank)
    {
        if (rank is < 1 or > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(rank));
        }
        
        File = CharFileToInt(file);
        Rank = rank - 1;
    }

    private static int CharFileToInt(char file)
    {
        file = char.ToLowerInvariant(file);
        
        int index = Files.IndexOf(file);

        return index == -1 ? throw new ArgumentException("Invalid file") : index;
    }

    public override string ToString()
    {
        return Files[File].ToString() + (Rank + 1);
    }
}