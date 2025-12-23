namespace ChessServer.Api.Domain.Match;

public class MatchConnection
{
    public string UserId { get; init; }
    public Guid MatchId { get; init; }
    public bool IsActive { get; set; } = true;
}