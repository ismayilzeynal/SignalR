namespace FrontToBack.Models
{
    public class Sales
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public double TotalPrice { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<SalesProducts> SalesProducts { get; set; }
    }
}
