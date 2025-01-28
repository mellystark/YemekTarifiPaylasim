using Microsoft.EntityFrameworkCore;
using YemekTarifleriUygulamasi.Models;

namespace YemekTarifleriUygulamasi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Kullanıcılar (Users) tablosu
        public DbSet<User> Users { get; set; }

        // Tarifler (Recipes) tablosu
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Comment> Comments { get; set; }

        // Fluent API kullanarak ilişkileri tanımlama (Optional)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Recipe ve User arasındaki ilişkiyi tanımlıyoruz
            modelBuilder.Entity<Recipe>()
                .HasOne(r => r.User)  // Her tarifin bir kullanıcıya ait olduğunu belirtiyoruz
                .WithMany(u => u.Recipes) // Her kullanıcının birden fazla tarifi olabileceğini belirtiyoruz
                .HasForeignKey(r => r.UserId) // Tarifteki UserId'nin ForeignKey olduğunu belirtiyoruz
                .OnDelete(DeleteBehavior.SetNull); // Eğer kullanıcı silinirse tarifler silinmesin, UserId null yapılır
        }
    }
}
