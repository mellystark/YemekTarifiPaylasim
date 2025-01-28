using System.Security.Claims;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using YemekTarifleriUygulamasi.Data;
using YemekTarifleriUygulamasi.Models;

namespace YemekTarifleriUygulamasi.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AccountService> _logger;

        public AccountService(AppDbContext context, ILogger<AccountService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public bool RegisterUser(User user)
        {
            if (IsUsernameTaken(user.Username))
            {
                return false;
            }
            try
            {
                user.Password = HashPassword(user.Password);
                _context.Users.Add(user);
                _context.SaveChanges();

                _logger.LogInformation("Yeni kullanıcı kaydedildi: {Username}", user.Username);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı kaydedilirken bir hata oluştu.");
                return false;
            }
        }

        public bool IsUsernameTaken(string username)
        {
            return _context.Users.Any(u => u.Username == username);
        }

        public bool IsEmailTaken(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        public User AuthenticateUser(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                _logger.LogInformation("Kullanıcı doğrulandı: {Username}", username);

                user.Role = user.IsAdmin.GetValueOrDefault(false) ? "Admin" : "User";
                return user;
            }

            _logger.LogWarning("Kullanıcı doğrulama başarısız. Kullanıcı adı: {Username}", username);
            return null;
        }

        public async Task SignInUserAsync(HttpContext httpContext, User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.IsAdmin.GetValueOrDefault(false) ? "Admin" : "User")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddHours(2)
            };

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
            _logger.LogInformation("Kullanıcı oturum açtı: {Username}", user.Username);
        }

        public async Task SignOutUserAsync(HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("Kullanıcı oturumu kapattı.");
        }

        public User GetCurrentUser(HttpContext httpContext)
        {
            var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userId, out int id))
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == id);
                if (user != null)
                {
                    _logger.LogInformation("Geçerli kullanıcı alındı: {Username}", user.Username);
                    return user;
                }
            }

            _logger.LogWarning("Geçerli kullanıcı alınamadı.");
            return null;
        }

        public bool UpdateUser(User updatedUser)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == updatedUser.Id);
                if (user == null)
                {
                    _logger.LogWarning("Kullanıcı bulunamadı. ID: {UserId}", updatedUser.Id);
                    return false;
                }

                user.Username = updatedUser.Username;
                user.Email = updatedUser.Email;
                user.IsAdmin = updatedUser.IsAdmin;
                user.ProfileImage = updatedUser.ProfileImage;

                if (!string.IsNullOrEmpty(updatedUser.Password) && updatedUser.Password != user.Password)
                {
                    _logger.LogInformation("Eski şifre hash'i: {OldPasswordHash}", user.Password);
                    user.Password = HashPassword(updatedUser.Password);
                    _logger.LogInformation("Yeni şifre hash'i: {NewPasswordHash}", user.Password);
                }

                _context.Users.Update(user);
                _context.SaveChanges();

                _logger.LogInformation("Kullanıcı güncellendi: {Username}", user.Username);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı güncellenirken bir hata oluştu.");
                return false;
            }
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
