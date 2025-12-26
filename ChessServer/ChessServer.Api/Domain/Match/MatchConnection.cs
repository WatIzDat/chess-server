using ChessServer.Api.Database;

namespace ChessServer.Api.Domain.Match;

public class MatchConnection
{
    public ApplicationUser User { get; init; }
    public string UserId { get; init; } = null!;
    public Guid MatchId { get; init; }
    public bool IsActive { get; set; } = true;
    public MatchPlayerType PlayerType { get; set; }

    public MatchConnection()
    {
    }

    public MatchConnection(ApplicationUser user, MatchPlayerType playerType)
    {
        User = user;
        PlayerType = playerType;
    }
}