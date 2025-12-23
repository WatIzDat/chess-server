using ChessServer.Api.Database;
using ChessServer.Api.Domain.Game;
using ChessServer.Api.Domain.Match;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChessServer.Api.Hubs;

[Authorize]
public class GameHub(ApplicationDbContext dbContext) : Hub
{
    private readonly ApplicationDbContext dbContext = dbContext;

    public async Task SendMove(string move)
    {
        Move newMove = Move.Create(move);
        
        await Clients.All.SendAsync("ReceiveMove", newMove.Serialize());
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        List<MatchConnection> connections = await dbContext.MatchConnections.Where(connection => connection.IsActive && connection.UserId == Context.UserIdentifier).ToListAsync();

        foreach (MatchConnection matchConnection in connections)
        {
            matchConnection.IsActive = false;
        }
        
        await dbContext.SaveChangesAsync();
            
        await base.OnDisconnectedAsync(exception);
    }
}