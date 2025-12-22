using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using ChessServer.ConsoleFrontend.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.SignalR.Client;

JsonSerializerOptions jsonOptions = new()
{
    PropertyNameCaseInsensitive = true
};

using HttpClient client = new();

AccessTokenResponse accessTokenResponse;

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
    
    Console.WriteLine(content);
    
    HttpResponseMessage response = await client.PostAsync("http://localhost:5075/login", content);
    
    response.EnsureSuccessStatusCode();
    
    accessTokenResponse = await response.Content.ReadFromJsonAsync<AccessTokenResponse>(jsonOptions) ?? throw new Exception("Login failed");

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
    JsonWebToken jwt = new JsonWebTokenHandler().ReadJsonWebToken(accessTokenResponse.AccessToken);

    if (DateTime.Now <= jwt.ValidTo.Subtract(TimeSpan.FromMinutes(1))) return accessTokenResponse.AccessToken;
    
    HttpResponseMessage response = await client.PostAsJsonAsync("http://localhost:5075/refresh", accessTokenResponse.RefreshToken);
        
    response.EnsureSuccessStatusCode();
        
    accessTokenResponse = await response.Content.ReadFromJsonAsync<AccessTokenResponse>(jsonOptions) ?? throw new Exception("Refresh failed");
    
    Console.WriteLine($"AccessToken: {accessTokenResponse.AccessToken}");

    return accessTokenResponse.AccessToken;
}