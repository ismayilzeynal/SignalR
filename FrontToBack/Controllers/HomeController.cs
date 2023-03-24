using FrontToBack.DAL;
using FrontToBack.Services.Basket;
using FrontToBack.Services.Product;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;


namespace FrontToBack.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductGet _productGet;
        private readonly AppDbContext _appDbContext;
        
        public HomeController(AppDbContext appDbContext, IProductGet productGet)
        {
            _appDbContext = appDbContext;
            _productGet = productGet;
        }

        public IActionResult Index()
        {
            var products = _productGet.GetProducts();
            HomeVM homeVM = new HomeVM();   
            homeVM.Sliders = _appDbContext.Sliders.ToList();
            homeVM.SliderDetail = _appDbContext.SliderDetails.FirstOrDefault();
            homeVM.Categories = _appDbContext.Categories.ToList();
            homeVM.Products = _appDbContext.Products.Include(p=> p.ProductImages).ToList();

            return View(homeVM);
        }



    }
}