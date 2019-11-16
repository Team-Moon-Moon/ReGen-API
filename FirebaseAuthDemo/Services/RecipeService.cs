using FirebaseAuthDemo.Models;
using FirebaseAuthDemo.Utils.Custom_Exceptions;
using FirebaseAuthDemo.Utils.Validation.Http_Request_Body_Schemas;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IDatabaseService _dbClient;
        private readonly IValidationService _vClient;

        public RecipeService(IDatabaseService databaseService, IValidationService validationClient)
        {
            _dbClient = databaseService;
            _vClient = validationClient;
        }

        public async Task<Recipe> GetRecipeAsync(string recipeId)
        {
            try
            {
                var recipe = await _dbClient.GetRecipeAsync(recipeId);

                return recipe;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CreateRecipeAsync(string userId, RecipeForm recipeForm)
        {
            try
            {
                var recipe =
                    new Recipe
                    {
                        AuthorID = userId,
                        Name = recipeForm.Name,
                        Calories = recipeForm.Calories,
                        PrepTimeMinutes = recipeForm.PrepTimeMinutes,
                        Ingredients = recipeForm.Ingredients,
                        Steps = recipeForm.Steps,
                        ImageReferencePath = recipeForm.ImageReferencePath,
                        RootImagePath = recipeForm.RootImagePath
                    };

                await _dbClient.AddRecipeAsync(recipe);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateRecipeAsync(string userId, string recipeId, RecipeForm recipeForm)
        {
            try
            {
                var recipe = await GetRecipeAsync(recipeId);

                if (recipe == null)
                {
                    throw new NoResultsFoundException($"No recipe found for ID: {recipeId}");
                }

                if (userId == recipe.AuthorID)
                {
                    // To-do: use AutoMapper
                    recipe.Calories = recipeForm.Calories;
                    recipe.ImageReferencePath = recipeForm.ImageReferencePath;
                    recipe.Ingredients = recipeForm.Ingredients;
                    recipe.Name = recipeForm.Name;
                    recipe.PrepTimeMinutes = recipeForm.PrepTimeMinutes;
                    recipe.RootImagePath = recipeForm.RootImagePath;
                    recipe.Steps = recipeForm.Steps;
                    recipe.Tags = recipeForm.Tags;

                    await _dbClient.UpdateRecipeAsync(recipe);
                }
                else
                {
                    throw new UnauthorizedAccessException();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteRecipeAsync(string userId, string recipeId)
        {
            try
            {
                var recipe = await GetRecipeAsync(recipeId);

                if (recipe == null)
                {
                    throw new NoResultsFoundException($"No recipe found for ID: {recipeId}");
                }

                if (userId == recipe.AuthorID)
                {
                    await _dbClient.DeleteRecipeAsync(recipe.Key);
                }
                else
                {
                    throw new UnauthorizedAccessException();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
