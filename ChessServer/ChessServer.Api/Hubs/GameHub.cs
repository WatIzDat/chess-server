using System.Diagnostics;
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

        if (!match.Board.IsMoveLegal(newMove,
                connection.PlayerType == MatchPlayerType.WhitePlayer ? PlayerColor.White : PlayerColor.Black))
            return;

        long now = Stopwatch.GetTimestamp();
        long elapsedTime = now - match.LastTurnStartTimestamp;

        if (match.Board.PlayerToMove == PlayerColor.White)
        {
            match.WhiteTimeRemaining -= elapsedTime;
        }
        else
        {
            match.BlackTimeRemaining -= elapsedTime;
        }

        double timeRemainingSeconds =
            (double)(match.Board.PlayerToMove == PlayerColor.White
                ? match.WhiteTimeRemaining
                : match.BlackTimeRemaining) / Stopwatch.Frequency;

        if (match.WhiteTimeRemaining <= 0 || match.BlackTimeRemaining <= 0)
        {
            await Clients.Group(connection.MatchId.ToString())
                .SendAsync("ReceiveMove",
                    Fen.CreateFenFromBoard(match.Board),
                    GameResult.Flag,
                    timeRemainingSeconds,
                    now);

            return;
        }

        match.LastTurnStartTimestamp = now;

        GameResult gameResult = match.Board.MakeMove(newMove);
        
        Console.WriteLine(gameResult);

        string positionKey = match.Board.GetPositionKey();
        
        match.PositionKeyList.Add(positionKey);

        if (match.PositionKeyList.FindAll(p => p == positionKey).Count >= 3)
        {
            gameResult = GameResult.DrawByRepetition;
        }
        else if (match.Board.HalfmoveClock >= 100 && gameResult != GameResult.Checkmate)
        {
            gameResult = GameResult.DrawByFiftyMoveRule;
        }
        
        dbContext.Entry(match).State = EntityState.Modified;
        
        await dbContext.SaveChangesAsync();
        
        await Clients.Group(connection.MatchId.ToString())
            .SendAsync("ReceiveMove",
                Fen.CreateFenFromBoard(match.Board),
                gameResult,
                timeRemainingSeconds,
                Stopwatch.GetTimestamp());
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