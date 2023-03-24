using FrontToBack.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Controllers
{
    public class DemoController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IConfiguration _configuration;


        public DemoController(AppDbContext appDbContext, IConfiguration configuration)
        {
            _appDbContext = appDbContext;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            //var connectionString = _configuration.GetSection("ConnectionStrings:DefaultConnection").Value;

            var books = _appDbContext.Books
                .Include(b=>b.BookImages)
                .Include(b=>b.BookGenres)
                .ThenInclude(bg=>bg.Genre)
                .Include(b=>b.BookAuthors)
                .ThenInclude(ba=>ba.Author)
                .ThenInclude(a=>a.SocialPage)
                .ToList();
                



            return View(books);
        }
    }
}
