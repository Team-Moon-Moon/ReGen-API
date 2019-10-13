using FirebaseAuthDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Services
{
    public interface IDatabaseService
    {
        #region 'Recipe' Operations

        Task<Recipe> GetRecipeAsync(string recipeId);

        #endregion

        #region 'Favorites' Operations

        Task<IDictionary<string, bool>> GetUserFavoritesAsync(string userId);

        Task AddUserFavoriteAsync(string userId, string recipeId);

        Task DeleteUserFavoriteAsync(string userId, string recipeId);

        #endregion

        #region Recipe rating Operations

        Task<double> GetAverageRatingAsync(string recipeId);

        Task<int> GetUserRatingAsync(string userId, string recipeId);

        Task AddUserRatingAsync(string userId, string recipeId, int rating);

        Task UpdateUserRatingAsync(string userId, string recipeId, int newRating);

        Task DeleteUserRatingAsync(string userId, string recipeId);

        #endregion
    }
}
