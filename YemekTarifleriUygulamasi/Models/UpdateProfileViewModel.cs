namespace YemekTarifleriUygulamasi.Models
{
    public class UpdateProfileViewModel
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        // Şifre ve şifre onayı sadece kullanıcı yeni bir şifre girdiğinde geçerli olacak
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }

}
