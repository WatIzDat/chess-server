using ChessServer.Api.Database;
using ChessServer.Api.Domain.Match;

namespace ChessServer.Api.Domain.Matchmaking;

public class MatchmakingPool
{
    public required TimeControl TimeControl { get; init; }
    public Guid TimeControlId { get; init; }
    public List<ApplicationUser> Users { get; init; } = [];
}