using FrontToBack.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public ProductController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IActionResult Index()
        {
            var query = _appDbContext.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Category);

            ViewBag.ProductCount = query.Count();
            var products = query
                .Take(2)
                .ToList();
            return View(products);
        }

       public IActionResult LoadMore(int skip)
        {
            var products = _appDbContext.Products
                .Include(p=> p.ProductImages)
                .Include(p=> p.Category)
                .Skip(skip)
                .Take(2)
                .ToList();
            return PartialView("_ProductLoadMorePartial", products);
        }
    }
}
