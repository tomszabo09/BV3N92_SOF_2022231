using Microsoft.AspNetCore.SignalR;

namespace Backend.Hubs
{
    public class ChatHub : Hub
    {
        public string GetConnectionId() => Context.ConnectionId;
    }
}
