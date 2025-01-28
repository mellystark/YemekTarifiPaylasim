using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YemekTarifleriUygulamasi.Models;
using YemekTarifleriUygulamasi.Services;
using Microsoft.AspNetCore.Authorization;

namespace YemekTarifleriUygulamasi.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // Kullanıcı kaydı için GET isteği
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Kullanıcı kaydı için POST isteği
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                // E-posta kontrolü
                if (_accountService.IsEmailTaken(user.Email) == true)
                {
                    ModelState.AddModelError("Email", "Bu e-posta adresi zaten kayıtlı.");
                    return View(user);
                }

                // Kullanıcı adı kontrolü (varsa)
                //if (!_accountService.IsUsernameTaken(user.Username))
                //{
                //    ModelState.AddModelError("Username", "Kullanıcı adı zaten alınmış.");
                //    return View(user);
                //}

                // Kullanıcı oluşturma işlemi
                var result = _accountService.RegisterUser(user);

                if (result)
                {
                    return RedirectToAction("Login"); // Başarılıysa login sayfasına yönlendir
                }
                else
                {
                    ViewData["ErrorMessage"] = "Bir hata oluştu. Lütfen tekrar deneyin.";
                    return View(user); // Hata varsa aynı sayfada tut
                }
            }

            return View(user); // ModelState geçersizse aynı sayfada kal
        }




        // Kullanıcı adı kontrolü
        [HttpGet]
        public JsonResult IsUsernameTaken(string username)
        {
            bool isTaken = _accountService.IsUsernameTaken(username);
            return Json(isTaken);
        }
        // E-posta kontrolü
        [HttpGet]
        public JsonResult IsEmailTaken(string email)
        {
            bool isTaken = _accountService.IsEmailTaken(email);
            return Json(isTaken);
        }
        
        // Kullanıcı giriş sayfası (GET isteği)
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Kullanıcı girişi işlemi (POST isteği)
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Server-side validation
            if (string.IsNullOrWhiteSpace(username))
            {
                ViewData["UsernameError"] = "Kullanıcı adı boş bırakılamaz.";
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ViewData["PasswordError"] = "Şifre boş bırakılamaz.";
            }

            // Eğer model geçerli değilse sayfayı yeniden yükle
            if (ViewData["UsernameError"] != null || ViewData["PasswordError"] != null)
            {
                return View();
            }

            // Kullanıcı doğrulama
            var user = _accountService.AuthenticateUser(username, password);

            if (user != null)
            {
                await _accountService.SignInUserAsync(HttpContext, user); // Kullanıcıyı oturum açtır

                // Kullanıcının admin olup olmadığını kontrol et
                if (user.Role == "Admin") // Kullanıcının rolü Admin ise
                {
                    return RedirectToAction("Index", "Admin"); // Admin controller'daki Index action'ına yönlendir
                }

                return RedirectToAction("Index", "Home");
            }


            // Kullanıcı doğrulama başarısız olursa hata mesajı döndür
            ViewData["ErrorMessage"] = "Kullanıcı adı veya şifre hatalı.";
            return View();
        }

        [HttpGet]
        public JsonResult IsEmailAvailable(string email)
        {
            // AccountService üzerinden kontrol et
            bool isAvailable = _accountService.IsEmailTaken(email);
            return Json(isAvailable);
        }
        // Kullanıcı çıkış işlemi (POST isteği)
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _accountService.SignOutUserAsync(HttpContext); // Kullanıcıyı oturumdan çıkart
            return RedirectToAction("Index", "Home");
        }

        // Profil sayfası (Giriş yapmış kullanıcılar için)
        [HttpGet]
        [Authorize]
        public IActionResult Profile()
        {
            var user = _accountService.GetCurrentUser(HttpContext); // Giriş yapan kullanıcıyı al

            if (user == null)
            {
                return RedirectToAction("Login"); // Giriş yapılmamışsa login sayfasına yönlendir
            }

            return View(user); // Kullanıcı profilini göster
        }
    }
}
