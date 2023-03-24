using FrontToBack.Models;

namespace FrontToBack.ViewModels
{
    public class SaleVM
    {
        public AppUser User { get; set; }
        public List<SalesProducts> SalesProducts { get; set; }
        public DateTime SaleDate { get; set; }
    }
}
