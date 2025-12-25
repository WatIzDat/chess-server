namespace ChessServer.Api.Domain.Game;

public record Square
{
    public int File { get; }
    public int Rank { get; }

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

    public Square? Up(int amount = 1)
    {
        return Rank + amount is < 0 or > 7 ? null : new Square(File, Rank + amount);
    }

    public Square? Down(int amount = 1)
    {
        return Rank - amount is < 0 or > 7 ? null : new Square(File, Rank - amount);
    }
    
    public Square? Right(int amount = 1)
    {
        return File + amount is < 0 or > 7 ? null : new Square(File + amount, Rank);
    }

    public Square? Left(int amount = 1)
    {
        return File - amount is < 0 or > 7 ? null : new Square(File - amount, Rank);
    }

    public static int CharFileToInt(char file)
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

public class SquareComparer : IComparer<Square>
{
    public int Compare(Square? x, Square? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (y is null) return 1;
        if (x is null) return -1;
        int rankComparison = x.Rank.CompareTo(y.Rank);
        if (rankComparison != 0) return rankComparison;
        return x.File.CompareTo(y.File);
    }
}