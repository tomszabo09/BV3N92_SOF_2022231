using Backend.Data;
using Backend.Data.Migrations;
using Backend.Hubs;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ChatController : Controller
    {
        IHubContext<ChatHub> _hub;
        private readonly UserManager<SiteUser> _userManager;

        public ChatController(IHubContext<ChatHub> hub, UserManager<SiteUser> userManager)
        {
            _hub = hub;
            _userManager = userManager;
        }

        [HttpPost("[action]/{connectionId}/{roomName}")]
        public async Task<IActionResult> JoinRoom(string connectionId, string roomName)
        {
            await _hub.Groups.AddToGroupAsync(connectionId, roomName);
            return Ok();
        }

        [HttpPost("[action]/{connectionId}/{roomName}")]
        public async Task<IActionResult> LeaveRoom(string connectionId, string roomName)
        {
            await _hub.Groups.RemoveFromGroupAsync(connectionId, roomName);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessage(string chatId, string message, string roomName, [FromServices] ApplicationDbContext ctx)
        {
            var user = await _userManager.GetUserAsync(User);

            var messageModel = new MessageModel()
            {
                ChatId = chatId,
                Text = message,
                Name = user.FirstName + ":",
                Timestamp = DateTime.Now
            };

            ctx.Messages.Add(messageModel);
            await ctx.SaveChangesAsync();

            await _hub.Clients.Group(roomName).SendAsync("RecieveMessage", message);

            return Ok();
        }
    }
}
