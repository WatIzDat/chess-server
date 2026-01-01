using ChessServer.Api.Domain;
using ChessServer.Api.Domain.Game;
using ChessServer.Api.Domain.Match;
using ChessServer.Api.Domain.Matchmaking;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ChessServer.Api.Database;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Match> Matches { get; set; }
    public DbSet<MatchConnection> MatchConnections { get; set; }
    public DbSet<TimeControl> TimeControls { get; set; }
    public DbSet<MatchmakingPool> MatchmakingPools { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.HasDefaultSchema("public");
        
        // builder.Entity<Match>()
        //     .ToTable("matches")
        //     .HasMany(e => e.ConnectedUsers)
        //     .WithMany()
        //     .UsingEntity<MatchConnection>(
        //         r => r.HasOne<ApplicationUser>().WithMany().HasForeignKey(e => e.UserId),
        //         l => l.HasOne<Match>().WithMany(e => e.Connections).HasForeignKey(e => e.MatchId),
        //         j => j.Property(e => e.IsActive).HasDefaultValue(true));

        builder.Entity<Match>()
            .HasMany(e => e.Connections)
            .WithOne()
            .HasForeignKey(e => e.MatchId);

        builder.Entity<Match>()
            .Property(e => e.Board)
            .HasConversion(
                v => Fen.CreateFenFromBoard(v),
                v => Fen.CreateBoardFromFen(v));
        
        builder.Entity<MatchConnection>()
            .HasKey(e => new { e.UserId, e.MatchId });

        builder.Entity<MatchConnection>()
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId);
        
        builder.Entity<MatchConnection>()
            .Property(e => e.IsActive)
            .HasDefaultValue(true);
        
        TimeSpanToTicksConverter timeSpanToTicksConverter = new();

        builder.Entity<TimeControl>()
            .Property(e => e.InitialTime)
            .HasConversion(timeSpanToTicksConverter);
        
        builder.Entity<TimeControl>()
            .Property(e => e.IncrementTime)
            .HasConversion(timeSpanToTicksConverter);

        builder.Entity<MatchmakingPool>()
            .HasKey(e => e.TimeControlId);

        builder.Entity<MatchmakingPool>()
            .HasOne(e => e.TimeControl)
            .WithMany()
            .HasForeignKey(e => e.TimeControlId);

        builder.Entity<MatchmakingPool>()
            .HasMany(e => e.Users)
            .WithMany();
    }
}