using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FrontToBack.Controllers
{
    [Area("AdminArea")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index(string search)
        {
            var users = search != null ? _userManager.Users
                .Where(u => u.Fullname.ToLower().Contains(search.ToLower()))
                .ToList():
                _userManager.Users
                .ToList();


            return View(users);
        }

        public async Task<IActionResult> Detail(string Id) 
        {
            if (Id == null) return NotFound();
            AppUser user = await _userManager.FindByIdAsync(Id);
            if(user == null) return NotFound();
            return View(new UserDetailVM
            {
                User=user,
                UserRoles= await _userManager.GetRolesAsync(user)
            });
        }

        public ActionResult BlockedUsers() 
        { 
            return View(_userManager.Users.Where(u=>!u.IsActive));
        }

        public async Task<IActionResult> EditRole(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);
            if(user == null ) return NotFound();

            return View(new ChangeRoleVM 
            {
                User = user,
                UserRoles = await _userManager.GetRolesAsync(user),
                AllRoles = _roleManager.Roles.ToList()
            });
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EditRole(string id, List<string> roles)
        {
            if(id == null) return NotFound();
            AppUser user = await _userManager.FindByIdAsync(id);
            if(user == null ) return NotFound();
            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user,userRoles);
            await _userManager.AddToRolesAsync(user, roles);


            return RedirectToAction("index", "user");
        }
    }
}
