using FirebaseAuthDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Services
{
    public interface IDatabaseService
    {
        #region 'User' Operations



        #endregion

        #region 'Recipe' Operations

        Task<Recipe> GetRecipeAsync(string recipeId);

        Task<Recipe> AddRecipeAsync(Recipe recipe);

        Task UpdateRecipeAsync(Recipe recipe);

        Task DeleteRecipeAsync(string recipeID);

        #endregion

        #region 'Favorites' Operations

        //Task<IDictionary<string, bool>> GetUserFavoritesAsync(string userId);
        Task<IEnumerable<string>> GetUserFavoritesAsync(string userId);

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

        #region 'Reviews' Operations

        Task<Review> GetUserReviewAsync(string userId, string recipeId);

        Task<ReviewsPage> GetReviewsPageAsync(string recipeId, string pageStartKey, int pageSize);

        Task AddReviewAsync(Review review);

        Task UpdateReviewAsync(Review review);

        Task DeleteReviewAsync(string userId, string recipeId);

        #endregion
    }
}
