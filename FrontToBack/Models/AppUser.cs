using Microsoft.AspNetCore.Identity;

namespace FrontToBack.Models
{
    public class AppUser:IdentityUser
    {
        public string Fullname { get; set; }
        public bool IsActive { get; set; }
        public List<Sales> Sales { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
