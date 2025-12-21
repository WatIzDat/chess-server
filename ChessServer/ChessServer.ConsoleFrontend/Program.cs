using Microsoft.AspNetCore.SignalR.Client;

HubConnection gameHubConnection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5075/gameHub")
    .WithAutomaticReconnect()
    .Build();

gameHubConnection.On<string>("ReceiveMove", move => Console.WriteLine($"Receieved move: {move}"));

await gameHubConnection.StartAsync();

while (true)
{
    Console.WriteLine("Move: ");
    string move = Console.ReadLine()!;
    
    await gameHubConnection.InvokeAsync("SendMove", move);
}