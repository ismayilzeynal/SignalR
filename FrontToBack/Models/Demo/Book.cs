using System.ComponentModel.DataAnnotations.Schema;

namespace FrontToBack.Models.Demo
{
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<BookImages> BookImages { get; set; }
        public List<BookGenre> BookGenres { get; set; }
        public List<BookAuthor> BookAuthors { get; set; }

        // Silmek
        [NotMapped]
        public List<int> GenreIds { get; set; }
        [NotMapped]
        public List<int> AuthorIds { get; set; }

    }
}
