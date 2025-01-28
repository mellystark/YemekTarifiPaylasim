using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Yetkilendirme i�in gerekli
using YemekTarifleriUygulamasi.Models;
using Microsoft.EntityFrameworkCore;
using YemekTarifleriUygulamasi.Data;

namespace YemekTarifleriUygulamasi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Gizlilik politikas�
        public IActionResult Privacy()
        {
            return View();
        }

        private List<Category> GetCategories()
        {
            return new List<Category>
            {
                new Category { Name = "Tatl�", ImageUrl = "/images/category/tatli.png" },
                new Category { Name = "Vejetaryen", ImageUrl = "/images/category/vejetaryen.jpg" },
                new Category { Name = "�orba", ImageUrl = "/images/category/corba.webp" },
                new Category { Name = "Aperatif", ImageUrl = "/images/category/aperatif.jpg" },
                new Category { Name = "Tavuk", ImageUrl = "/images/category/tavuk.webp" },
                new Category { Name = "Salata - Meze", ImageUrl = "/images/category/salata.webp" },
                new Category { Name = "Makarna", ImageUrl = "/images/category/makarna.webp" },
                new Category { Name = "Et", ImageUrl = "/images/category/et.webp" },
                new Category { Name = "Fast Food", ImageUrl = "/images/category/fastfood.webp" },
                
            };
        }

        
        public IActionResult Index()
        {
            var latestRecipes = _context.Recipes
                            .OrderByDescending(r => r.EklenmeTarihi)
                            .Take(6)
                            .ToList();

            // Tariflerin �ef bilgisiyle birlikte al�nmas�
            var recipesWithUsers = latestRecipes.Select(recipe => new
            {
                Recipe = recipe,
                User = _context.Users.FirstOrDefault(u => u.Id == recipe.UserId)
            }).ToList();

            var categories = GetCategories();
            ViewBag.Categories = categories;

            ViewBag.LatestRecipes = recipesWithUsers;
            return View();
        }

        // Tarif detaylar� i�in �ef bilgisi ile birlikte id g�nderme
        
        public IActionResult RecipeDetails(int id)
        {
            var recipe = _context.Recipes
                .FirstOrDefault(r => r.Id == id);

            if (recipe == null)
            {
                return NotFound();
            }

            // Tarifin �ef bilgisiyle birlikte detaylar� g�nder
            var user = _context.Users.FirstOrDefault(u => u.Id == recipe.UserId);
            ViewBag.Chef = user?.Username;  // �efin ismini View'a ta��yoruz

            return RedirectToAction("Details", "Recipe", new { id = recipe.Id });
        }


        public IActionResult CategoryRecipes(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                return NotFound("Kategori ad� ge�ersiz.");
            }

            // Veritaban�ndan ilgili kategorideki tarifleri getir ve �ef bilgisiyle birlikte y�kle
            var recipesWithUsers = _context.Recipes
                .Where(r => r.Category == categoryName)
                .Join(
                    _context.Users,
                    recipe => recipe.UserId,
                    user => user.Id,
                    (recipe, user) => new RecipeWithUserViewModel { Recipe = recipe, User = user }
                )
                .ToList();

            // E�er kategoriye ait tarif yoksa hata d�nd�r
            if (!recipesWithUsers.Any())
            {
                return NotFound($"'{categoryName}' kategorisine ait tarif bulunamad�.");
            }

            // Kategori ismini View'a ta��mak i�in
            ViewBag.CategoryName = categoryName;

            // Modeli View'a g�nderiyoruz
            return View(recipesWithUsers);
        }

        // T�m tarifleri g�sterme
        public IActionResult AllRecipes()
        {
            var recipesWithUsers = _context.Recipes
                .Include(r => r.User) // �ef bilgisi ile birlikte tarifleri getir
                .Select(r => new RecipeWithUserViewModel
                {
                    Recipe = r,
                    User = r.User
                })
                .ToList();

            return View(recipesWithUsers); // Yeni ViewModel ile g�nder
        }


        //Arama fonksiyonu
        [HttpPost]
        public IActionResult Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return RedirectToAction("Index");
            }

            var results = _context.Recipes
                .Where(r => r.Title.Contains(searchTerm) || r.Description.Contains(searchTerm))
                .ToList();

            return View("SearchResults", results);
        }




        // Hata sayfas�
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
