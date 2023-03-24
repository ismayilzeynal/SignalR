namespace FrontToBack.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set;}
    }
}
