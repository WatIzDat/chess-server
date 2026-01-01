using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ChessServer.ConsoleFrontend;
using ChessServer.ConsoleFrontend.Models;
using Microsoft.AspNetCore.SignalR.Client;

Console.OutputEncoding = System.Text.Encoding.UTF8;

JsonSerializerOptions jsonOptions = new()
{
    PropertyNameCaseInsensitive = true
};

using HttpClient client = new();

AccessTokenResponse accessTokenResponse;

DateTime tokenExpirationDate;

while (true)
{
    Console.Write("Choose action (register/login): ");
    string? authAction = Console.ReadLine();

    if (authAction == null || (authAction != "register" && authAction != "login"))
    {
        continue;
    }
    
    Console.Write("Email: ");
    string? email = Console.ReadLine();
    
    Console.Write("Password: ");
    string? password = Console.ReadLine();

    if (email == null || password == null)
    {
        continue;
    }

    HttpContent content = new StringContent(JsonSerializer.Serialize(new { email, password }), mediaType: new MediaTypeHeaderValue("application/json"));

    if (authAction.Equals("register", StringComparison.InvariantCultureIgnoreCase))
    {
        await client.PostAsync("http://localhost:5075/register", content);
    }
    
    HttpResponseMessage response = await client.PostAsync("http://localhost:5075/login", content);
    
    response.EnsureSuccessStatusCode();
    
    accessTokenResponse = await response.Content.ReadFromJsonAsync<AccessTokenResponse>(jsonOptions) ?? throw new Exception("Login failed");

    tokenExpirationDate = DateTime.UtcNow.AddSeconds(accessTokenResponse.ExpiresIn);
    
    break;
}

client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenResponse.AccessToken);

HubConnection gameHubConnection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5075/gameHub", options =>
    {
        options.AccessTokenProvider = GetAccessToken;
    })
    .WithAutomaticReconnect()
    .Build();

long serverTimestamp = 0;
long serverTimeOffset;

bool isWhiteToMove = true;

double whiteTime = 30;
double blackTime = 30;

await gameHubConnection.StartAsync();

Guid matchId;

CancellationTokenSource cancellationTokenSource = new();

gameHubConnection.On<int, long>("ReceiveJoin", (type, newServerTimestamp) =>
{
    Console.WriteLine("test");
    
    if (type == 1)
    {
        serverTimestamp = newServerTimestamp;
        
        long clientReceiveTime = Stopwatch.GetTimestamp();
        serverTimeOffset = serverTimestamp - clientReceiveTime;
        
        Console.WriteLine(serverTimeOffset);
        
        _ = RunTicker(cancellationTokenSource.Token);
    }
});

while (true)
{
    Console.Write("Choose action (create/join) match: ");
    string? matchAction = Console.ReadLine();

    if (matchAction == null || (matchAction != "create" && matchAction != "join"))
    {
        continue;
    }

    if (matchAction.Equals("join", StringComparison.InvariantCultureIgnoreCase))
    {
        while (true)
        {
            Console.Write("Match id: ");
        
            bool success = Guid.TryParse(Console.ReadLine(), out matchId);

            if (!success)
            {
                continue;
            }
            
            HttpResponseMessage response = await client.PatchAsync("http://localhost:5075/match/" + matchId, null);
            
            response.EnsureSuccessStatusCode();
            
            // MatchResponse matchResponse = await response.Content.ReadFromJsonAsync<MatchResponse>() ?? throw new Exception("Join match failed");
            
            // serverTimestamp = matchResponse.ServerTimestamp;
            //
            // long clientReceiveTime = Stopwatch.GetTimestamp();
            // serverTimeOffset = serverTimestamp - clientReceiveTime;

            await gameHubConnection.InvokeAsync("JoinMatch", matchId);

            break;
        }
    }
    else
    {
        Console.Write("FEN (leave empty for default position): ");
        string fen = Console.ReadLine()!;
        
        HttpContent content = new StringContent(JsonSerializer.Serialize(fen), mediaType: new MediaTypeHeaderValue("application/json"));
        
        HttpResponseMessage response = await client.PostAsync("http://localhost:5075/match", content);
        
        response.EnsureSuccessStatusCode();
        
        MatchResponse matchResponse = await response.Content.ReadFromJsonAsync<MatchResponse>() ?? throw new Exception("Create match failed");
        
        string stringMatchId = matchResponse.Id;
        
        Console.WriteLine($"Match id: {stringMatchId}");
        
        // serverTimestamp = matchResponse.ServerTimestamp;
        //
        // long clientReceiveTime = Stopwatch.GetTimestamp();
        // serverTimeOffset = serverTimestamp - clientReceiveTime;
        
        // Console.WriteLine("Offset: " + serverTimeOffset);
        
        matchId = Guid.Parse(stringMatchId);
        
        await gameHubConnection.InvokeAsync("JoinMatch", matchId);
    }

    break;
}

gameHubConnection.On<string, int, double, long>("ReceiveMove", (board, result, timeRemaining, newServerTimestamp) =>
{
    Console.WriteLine(Fen.FenToDisplayBoard(board));
    Console.WriteLine(result);
    Console.WriteLine(timeRemaining);

    if (isWhiteToMove)
        whiteTime = timeRemaining;
    else
        blackTime = timeRemaining;

    isWhiteToMove = !isWhiteToMove;

    serverTimestamp = newServerTimestamp;
    
    long clientReceiveTime = Stopwatch.GetTimestamp();
    serverTimeOffset = serverTimestamp - clientReceiveTime;
});


async Task RunTicker(CancellationToken cancellationToken)
{
    while (!cancellationToken.IsCancellationRequested)
    {
        long estimatedServerNow = Stopwatch.GetTimestamp() + serverTimeOffset;
        long elapsedTime = estimatedServerNow - serverTimestamp;

        double elapsedTimeSeconds = (double)elapsedTime / Stopwatch.Frequency;

        double whiteTimeRemaining = whiteTime;
        double blackTimeRemaining = blackTime;

        if (isWhiteToMove)
            whiteTimeRemaining -= elapsedTimeSeconds;
        else
            blackTimeRemaining -= elapsedTimeSeconds;
        
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write($"White: {whiteTimeRemaining:F}, Black: {blackTimeRemaining:F}");

        await Task.Delay(50, cancellationToken);
    }
}

while (true)
{
    Console.WriteLine("Move: ");
    string move = Console.ReadLine()!;
    
    await gameHubConnection.InvokeAsync("SendMove", move, matchId);
}

async Task<string?> GetAccessToken()
{
    if (DateTime.UtcNow <= tokenExpirationDate.Subtract(TimeSpan.FromMinutes(1))) return accessTokenResponse.AccessToken;
    
    HttpResponseMessage response = await client.PostAsJsonAsync("http://localhost:5075/refresh", accessTokenResponse.RefreshToken);
        
    response.EnsureSuccessStatusCode();
        
    accessTokenResponse = await response.Content.ReadFromJsonAsync<AccessTokenResponse>(jsonOptions) ?? throw new Exception("Refresh failed");
    
    tokenExpirationDate = DateTime.UtcNow.AddSeconds(accessTokenResponse.ExpiresIn);

    return accessTokenResponse.AccessToken;
}