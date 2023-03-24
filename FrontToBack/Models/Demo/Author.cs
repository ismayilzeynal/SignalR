namespace FrontToBack.Models.Demo
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SocialPageId { get; set; }
        public SocialPage SocialPage { get; set; }
        public List<BookAuthor> BookAuthors { get; set; }
    }
}
