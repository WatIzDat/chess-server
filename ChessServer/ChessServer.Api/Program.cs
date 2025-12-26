using System.Security.Claims;
using System.Web;
using ChessServer.Api.Database;
using ChessServer.Api.Domain.Game;
using ChessServer.Api.Domain.Match;
using ChessServer.Api.Extensions;
using ChessServer.Api.Hubs;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddApiEndpoints();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    app.ApplyMigrations();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/board/{fen}", (string fen) =>
{
    fen = HttpUtility.UrlDecode(fen);
    
    Board board = Fen.CreateBoardFromFen(fen);

    Console.WriteLine("Pieces:");
    foreach (var piece in board.Pieces)
    {
        Console.WriteLine(piece + " " + piece.Value.Color);
    }

    Console.WriteLine("Player to Move: " + board.PlayerToMove);

    Console.WriteLine("White kingside castling rights: " + board.CastlingRights[PlayerColor.White].Kingside);
    Console.WriteLine("White queenside castling rights: " + board.CastlingRights[PlayerColor.White].Queenside);
    Console.WriteLine("Black kingside castling rights: " + board.CastlingRights[PlayerColor.Black].Kingside);
    Console.WriteLine("Black queenside castling rights: " + board.CastlingRights[PlayerColor.Black].Queenside);

    Console.WriteLine("White en passant target square: " + board.EnPassantTargetSquares[PlayerColor.White]);
    Console.WriteLine("Black en passant target square: " + board.EnPassantTargetSquares[PlayerColor.Black]);

    Console.WriteLine("Halfmove clock: " + board.HalfmoveClock);
    
    return Fen.CreateFenFromBoard(board);
});

app.MapGet("/squares/{fen}/{square}", (string fen, string square) =>
{
    fen = HttpUtility.UrlDecode(fen);

    Board board = Fen.CreateBoardFromFen(fen);

    Square fromSquare = new(square[0], int.Parse(square[1].ToString()), true);

    foreach (Square legalSquare in board.Pieces[fromSquare].GetLegalSquares(fromSquare, board))
    {
        Console.WriteLine(legalSquare);
    }

    return Results.NoContent();
});

app.MapGet("/move/{fen}/{move}", (string fen, string move) =>
{
    fen = HttpUtility.UrlDecode(fen);

    Board board = Fen.CreateBoardFromFen(fen);

    Move newMove = Move.Create(move);

    return board.IsMoveLegal(newMove) ? "legal" : "illegal";
});

app.MapPost("/match", async (ClaimsPrincipal claims, ApplicationDbContext dbContext) =>
{
    string userId = claims.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

    ApplicationUser user = (await dbContext.Users.FindAsync(userId))!;

    Match match = new(user);
    
    dbContext.Matches.Add(match);
    await dbContext.SaveChangesAsync();

    return match.Id;
})
.RequireAuthorization();

app.MapPatch("/match/{matchId:guid}", async (ClaimsPrincipal claims, ApplicationDbContext dbContext, Guid matchId) =>
{
    string userId = claims.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

    ApplicationUser user = (await dbContext.Users.FindAsync(userId))!;

    Match? match = await dbContext.Matches.Include(match => match.ConnectedUsers).FirstOrDefaultAsync(match => match.Id == matchId);
    
    if (match == null)
    {
        return Results.NotFound();
    }

    match.ConnectedUsers.Add(user);

    await dbContext.SaveChangesAsync();

    return Results.Ok(match.Id);
})
.RequireAuthorization();

app.MapHub<GameHub>("/gameHub");

app.MapIdentityApi<ApplicationUser>();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
