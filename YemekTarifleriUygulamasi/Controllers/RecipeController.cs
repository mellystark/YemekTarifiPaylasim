using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using YemekTarifleriUygulamasi.Models;
using YemekTarifleriUygulamasi.Services;
using Microsoft.EntityFrameworkCore;
using YemekTarifleriUygulamasi.Data;

namespace YemekTarifleriUygulamasi.Controllers
{
    public class RecipeController : Controller
    {
        private readonly IRecipeService _recipeService;
        private readonly AppDbContext _context;

        public RecipeController(IRecipeService recipeService, AppDbContext context)
        {
            _recipeService = recipeService;
            _context = context;
        }

        public IActionResult Index()
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                ViewBag.Message = "Lütfen giriş yapın.";
                return View(new List<Recipe>());
            }

            // Kullanıcıya ait tarifleri alıyoruz
            var recipes = _recipeService.GetAllRecipes()
                .Where(r => r.UserId.ToString() == currentUserId)
                .ToList();

            if (!recipes.Any())
            {
                ViewBag.Message = "Henüz tarif eklenmemiş.";
            }

            return View(recipes);
        }

        public IActionResult Details(int id)
        {
            var recipe = _context.Recipes
             .Include(r => r.User) // Şef bilgisini dahil ediyoruz
             .Include(r => r.Comments) // Yorumları dahil ediyoruz
             .ThenInclude(c => c.User) // Yorumların kullanıcı bilgilerini dahil ediyoruz
             .FirstOrDefault(r => r.Id == id);

            if (recipe == null)
                return NotFound();

            return View(recipe);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_recipeService.GetCategories());
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Recipe recipe, IFormFile image)
        {
            try
            {
                // Kullanıcı kimliğini al (User.Identity.Name ile giriş yapan kullanıcının bilgisi)
                var user = _context.Users
                    .FirstOrDefault(u => u.Username == User.Identity.Name);

                // Eğer kullanıcı bulunmazsa bir hata fırlat
                if (user == null)
                {
                    ModelState.AddModelError("", "Kullanıcı bulunamadı.");
                    return View(recipe);
                }

                // Kullanıcıyı Recipe modeline ilişkilendir
                recipe.User = user; // Burada User nesnesini doğrudan ilişkilendiriyoruz
                if (ModelState.IsValid)
                {
                  

                    // Resmi yükle ve tarifi ekle
                    recipe.ImagePath = _recipeService.UploadImage(image);
                    _recipeService.AddRecipe(recipe);

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // ModelState geçerli değilse hata mesajlarını kontrol et
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        // Hata mesajlarını loglayabilir veya ekranda gösterebilirsiniz
                        Console.WriteLine(error.ErrorMessage); // Konsola yazdırma örneği
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            // Kategorileri yeniden yükle
            ViewBag.Categories = new SelectList(_recipeService.GetCategories(), recipe.Category);
            return View(recipe);
        }


        public IActionResult Edit(int id)
        {
            var recipe = _recipeService.GetRecipeById(id);
            if (recipe == null)
                return NotFound();

            ViewBag.Categories = new SelectList(_recipeService.GetCategories(), recipe.Category);
            return View(recipe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Recipe recipe, IFormFile image)
        {
            if (id != recipe.Id)
                return BadRequest();

            var existingRecipe = _recipeService.GetRecipeById(id);
            if (existingRecipe == null)
                return NotFound();

            try
            {
                string newImagePath = null;
                if (image != null)
                {
                    newImagePath = _recipeService.UploadImage(image);
                }

                _recipeService.UpdateRecipe(existingRecipe, recipe, newImagePath);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            ViewBag.Categories = new SelectList(_recipeService.GetCategories(), recipe.Category);
            return View(recipe);
        }

        public IActionResult Delete(int id)
        {
            var recipe = _recipeService.GetRecipeById(id);
            if (recipe == null)
                return NotFound();

            return View(recipe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(int recipeId, string content)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            var user = _context.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
                return Unauthorized();

            var recipe = _context.Recipes.FirstOrDefault(r => r.Id == recipeId);
            if (recipe == null)
                return NotFound();

            var comment = new Comment
            {
                Content = content,
                RecipeId = recipeId,
                UserId = user.Id,
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = recipeId });
        }



        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var recipe = _recipeService.GetRecipeById(id);
            if (recipe != null)
            {
                _recipeService.DeleteRecipe(recipe);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
