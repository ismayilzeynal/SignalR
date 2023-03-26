using FrontToBack.DAL;
using FrontToBack.Hubs;
using FrontToBack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.SignalR;

namespace FrontToBack.Controllers
{
    
    public class ChatController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(UserManager<AppUser> userManager, IHubContext<ChatHub> hubContext)
        {
            _userManager = userManager;
            _hubContext = hubContext;
        }

        public IActionResult Message()
        {
            ViewBag.Users = _userManager.Users.ToList();
            return View();
        }

        public async Task<IActionResult> ShowAlert(string UserId)
        {
            AppUser user = await _userManager.FindByIdAsync(UserId);
            _hubContext.Clients.Client(user.ConnectionId).SendAsync("ShowAlert", "hello");
            // _hubContext.Clients.Clients() // toplu, qrup
            return RedirectToAction("message");
        }
    }
}