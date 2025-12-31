namespace ChessServer.ConsoleFrontend.Models;

public class MatchResponse
{
    public required string Id { get; init; }
    public long ServerTimestamp { get; init; }
}