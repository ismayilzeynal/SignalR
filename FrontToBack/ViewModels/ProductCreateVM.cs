using FrontToBack.Models;

namespace FrontToBack.ViewModels
{
    public class ProductCreateVM
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int CategoryID { get; set; }
        public IFormFile[] Photos { get; set; }
    }
}
