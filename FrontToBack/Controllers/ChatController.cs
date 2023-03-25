using Microsoft.AspNetCore.Mvc;

namespace FrontToBack.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Message()
        {
            return View();
        }
    }
}
