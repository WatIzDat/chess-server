using ChessServer.Api.Database;
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

            IEnumerable<Task> tasks = poolIds.Select(id => RunMatchmakingPassAsync(id, dbContext, stoppingToken));
            
            await Task.WhenAll(tasks);
        }

        async Task RunMatchmakingPassAsync(Guid poolId, ApplicationDbContext dbContext, CancellationToken cancellationToken)
        {
            Console.WriteLine(poolId);

            List<ApplicationUser> users = (await dbContext.MatchmakingPools
                .Include(p => p.Users)
                .FirstAsync(p => p.TimeControlId == poolId, cancellationToken))
                .Users;

            HashSet<ApplicationUser> matchedUsers = [];

            foreach (ApplicationUser user in users)
            {
                ApplicationUser? matchedUser = users
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
                
                Console.WriteLine($"Matched {user} with {matchedUser}");
            }
        }
    }
}