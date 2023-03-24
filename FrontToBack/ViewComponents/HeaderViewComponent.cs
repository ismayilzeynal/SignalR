using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FrontToBack.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;

        public HeaderViewComponent(AppDbContext appDbContext, UserManager<AppUser> userManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.FullName=string.Empty;
            if (User.Identity.IsAuthenticated)
            {
                AppUser user =await _userManager.FindByNameAsync(User.Identity.Name);
                ViewBag.FullName = user.Fullname;

            }
            //string basket = Request.Cookies["basket"];
            //List<BasketVM> products;
            //if (basket == null)
            //{
            //    products = new();
            //}
            //else
            //{
            //    products = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
            //}

            //ViewBag.BasketProductCount = products.Count();
            Bio bio = _appDbContext.Bios.FirstOrDefault();
            return View(await Task.FromResult(bio));
        }

    }
}
