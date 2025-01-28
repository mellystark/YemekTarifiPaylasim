using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using YemekTarifleriUygulamasi.Models;

namespace YemekTarifleriUygulamasi.Services
{
    public interface IRecipeService
    {
        List<Recipe> GetAllRecipes();
        Recipe GetRecipeById(int id);
        List<string> GetCategories();
        string UploadImage(IFormFile image);
        void AddRecipe(Recipe recipe);
        void UpdateRecipe(Recipe existingRecipe, Recipe updatedRecipe, string newImagePath = null);
        void DeleteRecipe(Recipe recipe);
    }
}
