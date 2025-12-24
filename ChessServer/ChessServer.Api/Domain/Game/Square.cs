namespace ChessServer.Api.Domain.Game;

public record Square
{
    public int File { get; private set; }
    public int Rank { get; private set; }

    private static readonly char[] Files = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'];

    public Square(int file, int rank, bool oneIndexed = false)
    {
        if (oneIndexed)
        {
            rank -= 1;
        }

        if (file is < 0 or > 7)
        {
            throw new ArgumentOutOfRangeException(nameof(file));
        }
        
        if (rank is < 0 or > 7)
        {
            throw new ArgumentOutOfRangeException(nameof(rank));
        }
        
        File = file;
        Rank = rank;
    }
    
    public Square(char file, int rank, bool oneIndexed = false) 
        : this(CharFileToInt(file), rank, oneIndexed)
    {
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