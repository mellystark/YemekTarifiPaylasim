using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace YemekTarifleriUygulamasi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [PasswordValidation(ErrorMessage = "Şifre en az bir rakam içermelidir.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "E-posta adresi gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        public string Email { get; set; }

        public string? FirstName { get; set; }  // Ad
        public string? LastName { get; set; }   // Soyad
        public string? ProfileImage { get; set; }  // Profil Fotoğrafı
        public ICollection<Recipe>? Recipes { get; set; }

        public bool? IsAdmin { get; set; }
        public string? Role { get; set; }
    }

    public class PasswordValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return false;

            string password = value.ToString();
            return Regex.IsMatch(password, @"\d");
        }
    }
}
