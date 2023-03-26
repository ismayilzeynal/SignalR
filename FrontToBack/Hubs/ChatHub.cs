using FrontToBack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace FrontToBack.Hubs
{
    public class ChatHub:Hub
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public ChatHub(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public async Task SendMessage(string user, string message) // old version
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendMessage2(string[] users, string message)
        {

            foreach (var item in users)
            {
                AppUser User = await _userManager.FindByIdAsync(item);
                await Clients.Client(User.ConnectionId).SendAsync("ReceiveMessage2", Context.User.Identity.Name, message); //user
            }
        }

        public override async Task<Task> OnConnectedAsync()
        {
            if(Context.User.Identity.IsAuthenticated)
            {
                AppUser user = _userManager.FindByNameAsync(Context.User.Identity.Name).Result;
                user.ConnectionId = Context.ConnectionId;
                var result = _userManager.UpdateAsync(user).Result;

                await Clients.All.SendAsync("UserOnline",user.Id);

            }
            return base.OnConnectedAsync();
        }

        public override async Task<Task> OnDisconnectedAsync(Exception? exception)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                AppUser user = _userManager.FindByNameAsync(Context.User.Identity.Name).Result;
                user.ConnectionId = null;
                var result = _userManager.UpdateAsync(user).Result;
                await Clients.All.SendAsync("UserOffline", user.Id);
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}

