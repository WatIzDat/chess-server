namespace ChessServer.ConsoleFrontend.Models;

public class CreateMatchResponse
{
    public required string Id { get; init; }
    public long ServerTimestamp { get; init; }
}