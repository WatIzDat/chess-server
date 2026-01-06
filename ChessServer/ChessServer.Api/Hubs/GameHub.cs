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
            await UpdatePlayerRatingsAsync(GameResult.Flag);

            await dbContext.SaveChangesAsync();

            await Clients.Group(connection.MatchId.ToString())
                .SendAsync("ReceiveMove",
                    Fen.CreateFenFromBoard(match.Board),
                    GameResult.Flag,
                    timeRemainingSeconds,
                    (now * 1000) / (double)Stopwatch.Frequency);

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

        if (gameResult != GameResult.None)
        {
            await UpdatePlayerRatingsAsync(gameResult);
        }
        
        dbContext.Entry(match).State = EntityState.Modified;
        
        await dbContext.SaveChangesAsync();
        
        await Clients.Group(connection.MatchId.ToString())
            .SendAsync("ReceiveMove",
                Fen.CreateFenFromBoard(match.Board),
                gameResult,
                timeRemainingSeconds,
                (Stopwatch.GetTimestamp() * 1000) / (double)Stopwatch.Frequency);
        
        return;

        async Task UpdatePlayerRatingsAsync(GameResult result)
        {
            ApplicationUser playerA = await dbContext.Users.FindAsync(Context.UserIdentifier) ?? throw new Exception("User was somehow null");
            ApplicationUser playerB = (await dbContext.MatchConnections.Include(c => c.User).FirstAsync(c =>
                c.MatchId == matchId && c.IsActive && c.UserId != Context.UserIdentifier && c.PlayerType ==
                (connection.PlayerType == MatchPlayerType.WhitePlayer
                    ? MatchPlayerType.BlackPlayer
                    : MatchPlayerType.WhitePlayer))).User;

            double playerAExpectedScore = 1 / (1 + Math.Pow(10, (playerB.Rating - playerA.Rating) / 400));
            double playerAScore = result is GameResult.Checkmate or GameResult.Flag ? 1.0 : 0.5;
            double playerARating = playerA.Rating + 32 * (playerAScore - playerAExpectedScore);
            
            double playerBExpectedScore = 1 / (1 + Math.Pow(10, (playerA.Rating - playerB.Rating) / 400));
            double playerBScore = result is GameResult.Checkmate or GameResult.Flag ? 0.0 : 0.5;
            double playerBRating = playerB.Rating + 32 * (playerBScore - playerBExpectedScore);

            playerA.Rating = playerARating;
            playerB.Rating = playerBRating;
        }
    }

    public async Task JoinMatch(Guid matchId)
    {
        if (Context.UserIdentifier == null)
        {
            Console.WriteLine("UserIdentifier == null");
            return;
        }
        
        await Groups.AddToGroupAsync(Context.ConnectionId, matchId.ToString());
        
        MatchConnection? connection = await dbContext.MatchConnections.Where(c => c.MatchId == matchId && c.UserId == Context.UserIdentifier).FirstOrDefaultAsync();

        if (connection == null)
        {
            Console.WriteLine("Connection is null");

            connection = new MatchConnection
            {
                MatchId = matchId,
                UserId = Context.UserIdentifier,
                PlayerType = MatchPlayerType.Spectator
            };
            
            await dbContext.MatchConnections.AddAsync(connection);
        }
        else if (!connection.IsActive)
        {
            connection.IsActive = true;
        }
        
        int playerCount = await dbContext.MatchConnections.CountAsync(c =>
            c.MatchId == matchId &&
            c.IsActive &&
            (c.PlayerType == MatchPlayerType.WhitePlayer || c.PlayerType == MatchPlayerType.BlackPlayer));
        
        bool allPlayersJoined = playerCount == 2;

        Match match = (await dbContext.Matches.FindAsync(matchId))!;

        await dbContext.SaveChangesAsync();
        
        //await Clients.Caller.SendAsync("ReceiveJoin", connection.PlayerType, Stopwatch.GetTimestamp());
        
        await Clients.Group(matchId.ToString())
            .SendAsync("ReceiveJoin",
                connection.PlayerType,
                allPlayersJoined,
                match.WhiteTimeRemaining,
                match.BlackTimeRemaining,
                Fen.CreateFenFromBoard(match.Board),
                (Stopwatch.GetTimestamp() * 1000) / (double)Stopwatch.Frequency);
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