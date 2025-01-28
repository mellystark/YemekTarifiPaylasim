using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YemekTarifleriUygulamasi.Models;
using Microsoft.AspNetCore.Authorization;
using YemekTarifleriUygulamasi.Services;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YemekTarifleriUygulamasi.Data;

namespace YemekTarifleriUygulamasi.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IAccountService _accountService;

        private readonly AppDbContext _context;
        public ProfileController(AppDbContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        // GET: Profile page
        [HttpGet]
        [Authorize]
        public IActionResult Profile()
        {
            var user = _accountService.GetCurrentUser(HttpContext);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View("~/Views/Account/Profile.cshtml", user);
        }

        // POST: Upload Profile Picture
        [HttpPost]
        [Authorize]
        public IActionResult UploadProfileImage(IFormFile profileImage)
        {
            var user = _accountService.GetCurrentUser(HttpContext);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (profileImage != null && profileImage.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(profileImage.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    TempData["ErrorMessage"] = "Sadece JPG, PNG veya GIF dosyalari yukleyebilirsiniz.";
                    return RedirectToAction("Profile");
                }

                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                var uniqueFileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(uploads, uniqueFileName);

                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    profileImage.CopyTo(stream);
                }

                user.ProfileImage = "/images/" + uniqueFileName;
                _accountService.UpdateUser(user);

                TempData["SuccessMessage"] = "Profil fotografiniz basariyla guncellendi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Lutfen bir dosya secin.";
            }

            return RedirectToAction("Profile");
        }

        // POST: Update Profile
        // Kullanıcı bilgilerini güncellemek için POST metodu
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _accountService.GetCurrentUser(HttpContext);

                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Kullanıcı bilgilerini güncelle
                user.Username = model.Username;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;

                // Eğer şifreyi değiştirmek istiyorsa, şifreyi kontrol et
                if (!string.IsNullOrEmpty(model.Password)) // Yeni şifre girmişse
                {
                    if (model.Password == model.ConfirmPassword) // Şifre onayı eşleşiyor mu?
                    {
                        user.Password = _accountService.HashPassword(model.Password); // Şifreyi hash'le
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Yeni sifre ile sifreyi onaylama eslesmiyor.";
                        return RedirectToAction("Profile"); // Hata durumunda Profile sayfasına yönlendir
                    }
                }

                // Kullanıcıyı güncelle
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Profil bilgileriniz basariyla guncellendi.";
                return RedirectToAction("Profile");
            }

            TempData["ErrorMessage"] = "Lutfen gerekli tum bilgileri dogru sekilde doldurun.";
            return RedirectToAction("Profile");
        }
    }
}

