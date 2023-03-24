namespace FrontToBack.Models.Demo
{
    public class BookGenre
    {
        public int Id { get; set; }
        public int GenreID { get; set; }
        public Genre Genre { get; set; }
        public int BookID { get; set; }
        public Book Book { get; set; }

    }
}
