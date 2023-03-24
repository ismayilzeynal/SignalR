using FrontToBack.DAL;
using FrontToBack.ViewModels;
using Newtonsoft.Json;

namespace FrontToBack.Services.Product
{
    public class ProductGet : IProductGet
    {
        private readonly AppDbContext _appDbContext;

        public ProductGet(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        List<Models.Product> IProductGet.GetProducts()
        {
            var products = _appDbContext.Products.ToList();
            return products;
        }
    }
}
