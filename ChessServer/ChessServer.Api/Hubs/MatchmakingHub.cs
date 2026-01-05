using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChessServer.Api.Hubs;

[Authorize]
public class MatchmakingHub : Hub
{
}