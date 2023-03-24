namespace FrontToBack.Models
{
    public class SalesProducts
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }

        public int SalesId { get; set; }
        public Sales Sales { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

    }
}
