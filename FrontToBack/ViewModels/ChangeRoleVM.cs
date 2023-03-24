using FrontToBack.Models;
using Microsoft.AspNetCore.Identity;

namespace FrontToBack.ViewModels
{
    public class ChangeRoleVM
    {
        public AppUser User{ get; set; }
        public IList<string> UserRoles { get; set; }
        public List<IdentityRole> AllRoles { get; set; }
    }
}
