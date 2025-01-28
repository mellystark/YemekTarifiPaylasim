using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace YemekTarifleriUygulamasi.Models
{
    public class Recipe
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string? ImagePath { get; set; }

        [Required]
        public string Ingredients { get; set; }

        [Required]
        public string Steps { get; set; }

        [Required]
        public string Category { get; set; }

        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;

        // Kullanıcı ile ilişkilendirme
        public int? UserId { get; set; }
        public virtual User? User { get; set; }

        public List<Comment>? Comments { get; set; } = new List<Comment>();
    }
}