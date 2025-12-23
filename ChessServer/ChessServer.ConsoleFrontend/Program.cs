using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ChessServer.ConsoleFrontend.Models;
using Microsoft.AspNetCore.SignalR.Client;

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
        
            bool success = Guid.TryParse(Console.ReadLine(), out Guid matchId);

            if (!success)
            {
                continue;
            }
            
            HttpResponseMessage response = await client.PatchAsync("http://localhost:5075/match/" + matchId, null);
            
            response.EnsureSuccessStatusCode();

            break;
        }
    }
    else
    {
        HttpResponseMessage response = await client.PostAsync("http://localhost:5075/match", null);
        
        response.EnsureSuccessStatusCode();
    }

    break;
}

HubConnection gameHubConnection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5075/gameHub", options =>
    {
        options.AccessTokenProvider = GetAccessToken;
    })
    .WithAutomaticReconnect()
    .Build();

gameHubConnection.On<string>("ReceiveMove", move => Console.WriteLine($"Received move: {move}"));

await gameHubConnection.StartAsync();

while (true)
{
    Console.WriteLine("Move: ");
    string move = Console.ReadLine()!;
    
    await gameHubConnection.InvokeAsync("SendMove", move);
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