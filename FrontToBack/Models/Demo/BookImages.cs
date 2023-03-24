namespace FrontToBack.Models.Demo
{
    public class BookImages
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
