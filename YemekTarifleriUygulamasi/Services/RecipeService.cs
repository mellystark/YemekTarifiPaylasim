using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YemekTarifleriUygulamasi.Data;
using YemekTarifleriUygulamasi.Models;

namespace YemekTarifleriUygulamasi.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly AppDbContext _context;

        public RecipeService(AppDbContext context)
        {
            _context = context;
        }

        public List<Recipe> GetAllRecipes()
        {
            return _context.Recipes.OrderByDescending(r => r.EklenmeTarihi).ToList();
        }

        public Recipe GetRecipeById(int id)
        {
            return _context.Recipes.FirstOrDefault(r => r.Id == id);
        }

        public List<string> GetCategories()
        {
            return new List<string>
            {
                "Tatlı", "Vejetaryen", "Çorba", "Aperatif", "Tavuk", "Salata",
                "Makarna", "Et", "Fast Food", "Meze"
            };
        }

        public string UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return null;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(image.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException("Geçersiz dosya formatı.");

            var directory = Path.Combine("wwwroot", "images", "recipes");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(directory, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyTo(stream);
            }

            return $"/images/recipes/{fileName}";
        }

        public void AddRecipe(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
            _context.SaveChanges();
        }

        public void UpdateRecipe(Recipe existingRecipe, Recipe updatedRecipe, string newImagePath = null)
        {
            existingRecipe.Title = updatedRecipe.Title;
            existingRecipe.Description = updatedRecipe.Description;
            existingRecipe.Ingredients = updatedRecipe.Ingredients;
            existingRecipe.Steps = updatedRecipe.Steps;
            existingRecipe.Category = updatedRecipe.Category;
            existingRecipe.ImagePath = newImagePath ?? existingRecipe.ImagePath;

            _context.Recipes.Update(existingRecipe);
            _context.SaveChanges();
        }

        public void DeleteRecipe(Recipe recipe)
        {
            if (!string.IsNullOrEmpty(recipe.ImagePath))
            {
                var filePath = Path.Combine("wwwroot", recipe.ImagePath.TrimStart('/'));
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }

            _context.Recipes.Remove(recipe);
            _context.SaveChanges();
        }
    }
}
