using ChessServer.Api.Domain.Game;
using Microsoft.AspNetCore.SignalR;

namespace ChessServer.Api.Hubs;

public class GameHub : Hub
{
    public async Task SendMove(string move)
    {
        Move newMove = Move.Create(move);
        
        await Clients.All.SendAsync("ReceiveMove", newMove.Serialize());
    }
}