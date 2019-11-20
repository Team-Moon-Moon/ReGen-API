using FirebaseAuthDemo.Models;
using FirebaseAuthDemo.Utils.Validation.Http_Request_Body_Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Services
{
    public interface IRecipeService
    {
        Task<Recipe> GetRecipeAsync(string recipeId);

        Task<Recipe> CreateRecipeAsync(string userId, RecipeForm recipeForm);

        Task UpdateRecipeAsync(string userId, string recipeId, RecipeForm recipeForm);

        Task DeleteRecipeAsync(string userId, string recipeId); 
    }
}
