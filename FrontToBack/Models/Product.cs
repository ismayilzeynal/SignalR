namespace FrontToBack.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int CategoryID { get; set; }
        public Category Category { get; set; }
        public int Count { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public List<SalesProducts> SalesProducts { get; set; }
    }
}
