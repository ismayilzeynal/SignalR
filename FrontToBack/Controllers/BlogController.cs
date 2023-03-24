using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BlogController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> products = await _context.Products
                .Include(m => m.ProductImages)
                .Take(5)
                .ToListAsync();
            BlogVM blogVM = new()
            {
                Products = products,
                Blogs = _context.Blogs.ToList()
            };

            return View(blogVM);
        }

        public async Task<IActionResult> Detail(int id)
        {
            ViewBag.UserId = null;
            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                ViewBag.UserId = user.Id;
            }

            BlogDetailVM blogDetailVM = new();
            blogDetailVM.Blog = _context.Blogs
                .Include(bm => bm.Comments)
                .ThenInclude(c => c.AppUser)
                .FirstOrDefault(b => b.Id == id);
            blogDetailVM.Blogs = _context.Blogs.OrderByDescending(b => b.Id).Take(3).ToList();

            return View(blogDetailVM);
        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> AddComment(string commentMessage, int blogId)
        {
            AppUser user = null;

            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
                ViewBag.UserId = user.Id;
            }
            else
            {
                return RedirectToAction("login", "account");
            }

            Comment comment = new();
            comment.Message = commentMessage;
            comment.CreatedDate = DateTime.Now;
            comment.AppUserId = user.Id;
            comment.BlogId = blogId;
            _context.Comments.Add(comment);
            _context.SaveChanges();
            return RedirectToAction("Detail", new { id = blogId });
        }

        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = _context.Comments.FirstOrDefault(b=> b.Id == id);

            _context.Comments.Remove(comment);
            _context.SaveChanges();
            return RedirectToAction("detail", new {id = comment.BlogId });
        }
    }
}
