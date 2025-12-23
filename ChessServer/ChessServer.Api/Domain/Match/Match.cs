using ChessServer.Api.Database;

namespace ChessServer.Api.Domain.Match;

public class Match
{
    public Match()
    {
    }

    public Match(ApplicationUser initialUser)
    {
        ConnectedUsers = [initialUser];
    }
    
    public Guid Id { get; init; } = Guid.NewGuid();
    public List<ApplicationUser> ConnectedUsers { get; init; }
    public List<MatchConnection> Connections { get; } = [];
}