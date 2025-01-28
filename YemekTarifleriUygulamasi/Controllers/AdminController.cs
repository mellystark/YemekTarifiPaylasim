using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YemekTarifleriUygulamasi.Data;
using YemekTarifleriUygulamasi.Models;

namespace YemekTarifleriUygulamasi.Controllers
{
    [Authorize] // Sadece oturum açmış kullanıcılar erişebilir
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")] // Sadece admin kullanıcılar erişebilir
        [HttpGet]
        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            var recipes = _context.Recipes.ToList(); // Tarifleri çek
            return View(new AdminViewModel { Users = users, Recipes = recipes });
        }

        [HttpDelete]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Include(u => u.Recipes).FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                // Kullanıcıya ait tarifleri sil
                foreach (var recipe in user.Recipes)
                {
                    _context.Recipes.Remove(recipe); // Tarifleri sil
                }

                _context.Users.Remove(user); // Kullanıcıyı sil
                _context.SaveChanges();

                return Json(new { success = true, message = "Kullanıcı ve onun tarifleri başarıyla silindi." });
            }

            return Json(new { success = false, message = "Kullanıcı bulunamadı." });
        }

        [HttpDelete]
        public IActionResult DeleteRecipe(int id)
        {
            var recipe = _context.Recipes.Find(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe); // Tarif silme
                _context.SaveChanges();

                return Json(new { success = true, message = "Tarif başarıyla silindi." });
            }

            return Json(new { success = false, message = "Tarif bulunamadı." });
        }
    }

    public class AdminViewModel
    {
        public List<User> Users { get; set; }
        public List<Recipe> Recipes { get; set; }
    }
}
