using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using YemekTarifleriUygulamasi.Models;

namespace YemekTarifleriUygulamasi.Services
{
    public interface IAccountService
    {
        bool RegisterUser(User user);
        bool IsUsernameTaken(string username);
        bool IsEmailTaken(string email);
        User AuthenticateUser(string username, string password);
        Task SignInUserAsync(HttpContext httpContext, User user);
        Task SignOutUserAsync(HttpContext httpContext);
        User GetCurrentUser(HttpContext httpContext);
        bool UpdateUser(User updatedUser);
        string HashPassword(string password);
    }
}
