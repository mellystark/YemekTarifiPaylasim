namespace YemekTarifleriUygulamasi.Models
{
    public class UserProfile
    {
        public string Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; }
        public string? ProfileImage { get; set; } // Profil fotoğrafı için bir yol
    }
}
