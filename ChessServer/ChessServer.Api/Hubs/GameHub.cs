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

    public async Task SendMove(string move, Guid matchId)
    {
        Move newMove = Move.Create(move);

        MatchConnection? connection = await dbContext.MatchConnections.Where(c =>
            c.MatchId == matchId && c.UserId == Context.UserIdentifier && c.IsActive).FirstOrDefaultAsync();
        
        //List<MatchConnection> connections = await dbContext.MatchConnections.Where(connection => connection.IsActive && connection.UserId == Context.UserIdentifier).ToListAsync();
        
        if (connection == null ||
            !connection.PlayerType.IsPlayer())
            return;
        
        Match match = await dbContext.Matches.Where(m => m.Id == matchId).FirstAsync();

        if (!match.Board.IsMoveLegal(newMove, connection.PlayerType == MatchPlayerType.WhitePlayer ? PlayerColor.White : PlayerColor.Black))
            return;
        
        GameResult gameResult = match.Board.MakeMove(newMove);
        
        Console.WriteLine(gameResult);
        
        match.PositionKeyList.Add(match.Board.GetPositionKey());
        
        dbContext.Entry(match).State = EntityState.Modified;
        
        await dbContext.SaveChangesAsync();
        
        await Clients.Group(connection.MatchId.ToString())
            .SendAsync("ReceiveMove",
                Fen.CreateFenFromBoard(match.Board),
                gameResult);
    }

    public async Task JoinMatch(Guid matchId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, matchId.ToString());
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