using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.Services.Basket;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FrontToBack.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;

        public BasketController(AppDbContext appDbContext, UserManager<AppUser> userManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            //HttpContext.Session.SetString("name", "Ismayil");
            //Response.Cookies.Append("name", "Ismayil", new CookieOptions { MaxAge = TimeSpan.FromDays(1) });
            return Content("Set olundu");
        }

        public async Task<IActionResult> Add(int id, string name)
        {
            if (id == null) return NotFound();
            Product product = await _appDbContext.Products.FindAsync(id);

            if (product == null) return NotFound();
            List<BasketVM> products;

            if (Request.Cookies["basket"] == null)
            {
                products = new();
            }
            else
            {
                products = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
            }
            BasketVM existproduct = products.FirstOrDefault(p => p.Id == id);
            if (existproduct == null)
            {
                BasketVM basketVM = new();
                basketVM.Id = product.Id;
                basketVM.BasketCount = 1;
                basketVM.Price = product.Price;
                products.Add(basketVM);
            }
            else if(existproduct.BasketCount>=product.Count)
            {
                TempData["Error"] = $"{product.Name} adli mehsulun sayi tukenmisdir";
                RedirectToAction(nameof(Index), "Home");
            }
            else
            {
                existproduct.BasketCount++;
            }


            Response.Cookies.Append("basket",
                JsonConvert.SerializeObject(products), new CookieOptions { MaxAge = TimeSpan.FromHours(3) });

            return RedirectToAction(nameof(Index), "Home");
        }




        public IActionResult ShowBasket()
        {
            //string name = HttpContext.Session.GetString("name");
            //string name = Request.Cookies["name"];
            List<BasketVM> products;
            string basket = Request.Cookies["basket"];
            if (basket == null)
            {
                products = new();
            }
            else
            {
                products = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
                foreach (var item in products)
                {
                    Product currentProduct = _appDbContext.Products
                        .Include(p => p.ProductImages).FirstOrDefault(p => p.Id == item.Id);
                    item.Name = currentProduct.Name;
                    item.Price = currentProduct.Price;
                    item.Id = currentProduct.Id;
                    item.ImageUrl = currentProduct.ProductImages.FirstOrDefault().ImageUrl;

                }
            }
            return View(products);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Sale()
        {
            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                Sales sale = new();
                sale.AppUserId = user.Id;
                sale.CreatedDate = DateTime.Now;
                List<SalesProducts> salesProducts = new();
                List<BasketVM> basketVMs = 
                    JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
                foreach (var item in basketVMs)
                {
                    Product product = _appDbContext.Products.FirstOrDefault(p => p.Id == item.Id);

                    if(item.BasketCount>product.Count)
                    {
                        TempData["Error"] = $"{product.Name} adli mehsulun sayi tukenmisdir";
                        return RedirectToAction("showbasket");
                    }


                    SalesProducts salesProduct = new();
                    salesProduct.ProductId = product.Id;
                    salesProduct.SalesId = sale.Id;
                    salesProduct.Count = item.BasketCount;
                    salesProduct.Price = product.Price;
                    salesProducts.Add(salesProduct);

                    CountChangeAndSave(product, item);
                    product.Count -= item.BasketCount;
                }
                sale.SalesProducts = salesProducts;
                sale.TotalPrice = basketVMs.Sum(bp => bp.BasketCount*bp.Price);
                _appDbContext.Sales.Add(sale);
                _appDbContext.SaveChanges();
                TempData["Success"] = "satis ugurla yerine yetirildi";
                return RedirectToAction("ShowBasket");
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }



        private void CountChangeAndSave(Product product, BasketVM basketVM)
        {
            product.Count -= basketVM.BasketCount;
            _appDbContext.SaveChanges();
        }
    }
}
