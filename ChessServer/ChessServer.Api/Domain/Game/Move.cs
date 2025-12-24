namespace ChessServer.Api.Domain.Game;

public class Move(Square fromSquare, Square toSquare)
{
    public Square FromSquare { get; private set; } = fromSquare;
    public Square ToSquare { get; private set; } = toSquare;

    public static Move Create(string move)
    {
        if (move.Length != 4)
        {
            throw new ArgumentException("Move must be 4 characters long");
        }

        Square fromSquare = new(move[0], int.Parse(move[1].ToString()), true);
        Square toSquare = new(move[2], int.Parse(move[3].ToString()), true);
        
        return new Move(fromSquare, toSquare);
    }

    public string Serialize()
    {
        return FromSquare.ToString() + ToSquare;
    }
}