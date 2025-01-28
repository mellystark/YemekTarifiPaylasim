namespace YemekTarifleriUygulamasi.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int RecipeId { get; set; } // Yorumun ait olduğu tarifi bağlamak için
        public Recipe Recipe { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
