using Backend.Models;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Hubs
{
	public class ChatHub : Hub
	{
		public async Task SendMessage(MessageModel messageModel)
		{
			await Clients.All.SendAsync("receiveMessages", messageModel);
		}
	}
}
