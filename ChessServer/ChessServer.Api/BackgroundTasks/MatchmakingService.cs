using ChessServer.Api.Database;
using ChessServer.Api.Domain.Match;
using ChessServer.Api.Domain.Matchmaking;
using ChessServer.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChessServer.Api.BackgroundTasks;

public class MatchmakingService(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        PeriodicTimer timer = new(TimeSpan.FromMilliseconds(1000));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            
            ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            List<Guid> poolIds = await dbContext.MatchmakingPools.Select(p => p.TimeControlId).ToListAsync(stoppingToken);

            IEnumerable<Task> tasks = poolIds.Select(id => RunMatchmakingPassAsync(id, stoppingToken));
            
            await Task.WhenAll(tasks);
        }

        async Task RunMatchmakingPassAsync(Guid poolId, CancellationToken cancellationToken)
        {
            Console.WriteLine(poolId);
            
            using IServiceScope scope = serviceProvider.CreateScope();
            
            IHubContext<MatchmakingHub> hubContext = serviceProvider.GetRequiredService<IHubContext<MatchmakingHub>>();
            
            ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            MatchmakingPool pool = await dbContext.MatchmakingPools
                .Include(p => p.Users)
                .Include(p => p.TimeControl)
                .FirstAsync(p => p.TimeControlId == poolId, cancellationToken);

            HashSet<ApplicationUser> matchedUsers = [];

            foreach (ApplicationUser user in pool.Users.ToList())
            {
                ApplicationUser? matchedUser = pool.Users
                    .Where(u =>
                        u != user &&
                        !matchedUsers.Contains(u) &&
                        u.Rating >= user.Rating - 50 &&
                        u.Rating <= user.Rating + 50)
                    .OrderBy(u => Math.Abs(u.Rating - user.Rating))
                    .FirstOrDefault();

                if (matchedUser == null)
                    continue;
                
                matchedUsers.Add(user);
                matchedUsers.Add(matchedUser);

                Random random = new();
                bool isFirstUserWhitePlayer = random.Next(0, 2) == 0;

                Match match =
                    new(
                    [
                        new MatchConnection(user,
                            isFirstUserWhitePlayer ? MatchPlayerType.WhitePlayer : MatchPlayerType.BlackPlayer),
                        new MatchConnection(matchedUser,
                            isFirstUserWhitePlayer ? MatchPlayerType.BlackPlayer : MatchPlayerType.WhitePlayer)
                    ], pool.TimeControl.InitialTime.TotalSeconds);
                
                dbContext.Matches.Add(match);
                
                pool.Users.Remove(user);
                pool.Users.Remove(matchedUser);
                
                List<MatchmakingPool> queuedPools = await dbContext.MatchmakingPools
                    .Include(p => p.Users)
                    .Where(p => p.Users.Contains(user) || p.Users.Contains(matchedUser))
                    .ToListAsync(cancellationToken);

                foreach (MatchmakingPool p in queuedPools)
                {
                    p.Users.Remove(user);
                    p.Users.Remove(matchedUser);
                }
                
                await dbContext.SaveChangesAsync(cancellationToken);

                await hubContext.Clients.Users(user.Id, matchedUser.Id).SendAsync("MatchFound", match.Id, cancellationToken);
                
                Console.WriteLine($"Matched {user} with {matchedUser}");
            }
        }
    }
}