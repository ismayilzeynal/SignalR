using FrontToBack.DAL;
using FrontToBack.Extensions;
using FrontToBack.Helpers;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace FrontToBack.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    //[Authorize]
    public class ProductController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _env;
        public ProductController(AppDbContext appDbContext, IWebHostEnvironment env)
        {
            _appDbContext = appDbContext;
            _env = env;
        }

        public IActionResult Index(int page = 1, int take = 2)
        {
            var query = _appDbContext.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Category);

            var products = query.Skip((page - 1) * take)
              .Take(take)
              .ToList();

            int pageCount = CalculatePageCount(query, take);
            PaginationVM<Product> pagination = new(products, pageCount, page);
            return View(pagination);
        }

        private int CalculatePageCount(IIncludableQueryable<Product,Category> query, int take)
        {
            return (int)Math.Ceiling((decimal)(query.Count()) / take);
        }

        public IActionResult Create()
        {
            //ViewBag.Categories = _appDbContext.Categories.ToList();
            ViewBag.Categories = new SelectList(_appDbContext.Categories.ToList(), "Id", "Name");
            return View();
        }
        [HttpPost]
        public IActionResult Create(ProductCreateVM productCreateVM)
        {
            ViewBag.Categories = new SelectList(_appDbContext.Categories.ToList(), "Id", "Name");
            if (!ModelState.IsValid) return View();
            List<ProductImage> productImages = new();

            foreach (var photo in productCreateVM.Photos)
            {
                if (!photo.IsImage())
                {
                    ModelState.AddModelError("Photos", "Only images");
                    return View();
                }
                if (photo.CheckSize(500))
                {
                    ModelState.AddModelError("Photos", "Image size is large");
                    return View();
                }
                ProductImage productImage = new();
                productImage.ImageUrl = photo.SaveImage(_env, "img", photo.FileName);
                productImages.Add(productImage);
            }


            Product newProduct = new();
            newProduct.Name = productCreateVM.Name;
            newProduct.Price = productCreateVM.Price;
            newProduct.CategoryID = productCreateVM.CategoryID;
            newProduct.ProductImages = productImages;
            _appDbContext.Products.Add(newProduct);
            _appDbContext.SaveChanges();


            return RedirectToAction("Index");
        }





    }
}
