using FrontToBack.Models;

namespace FrontToBack.ViewModels
{
    public class UserDetailVM
    {
        public AppUser User { get; set; }
        public IList<string> UserRoles { get; set; }
    }
}
