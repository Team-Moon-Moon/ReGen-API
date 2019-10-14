using Firebase.Database;
using Firebase.Database.Query;
using FirebaseAuthDemo.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Services
{
    public class FirebaseDatabaseService : IDatabaseService
    {
        FirebaseClient _client;

        public FirebaseDatabaseService(FirebaseClient client)
        {
            _client = client;
        }

        #region Recipe CRUD operations

        public async Task<Recipe> GetRecipeAsync(string recipeId)
        {
            try
            {
                var result = await _client.Child("recipes")
                                          .Child(recipeId)
                                          .OnceSingleAsync<Recipe>();

                result.Key = recipeId;

                return result;
            }
            catch
            {
                throw; // Let the controller handle the exception.
            }
        }

        #endregion

        #region Favorites CRUD operations

        /// <summary>
        /// An asynchronous operation to retrieve a user's favorited recipes.
        /// The task result consists of dictionary of recipe ID keys and boolean 
        /// values (indicating if the recipe was favorited).
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IDictionary<string, bool>> GetUserFavoritesAsync(string userId)
        {
            try
            {
                var result = await _client.Child("users")
                                          .Child(userId)
                                          .Child("favorites")
                                          .OnceAsync<bool>();

                var favorites = result.ToDictionary(o => (o.Key),
                                                    o => (o.Object));

                return favorites;
            }
            catch
            {
                throw;
            }
        }

        public async Task AddUserFavoriteAsync(string userId, string recipeId)
        {
            try
            {
                await _client.Child("users")
                                 .Child(userId)
                                 .Child("favorites")
                                 .Child(recipeId)
                                 .PutAsync(true);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task DeleteUserFavoriteAsync(string userId, string recipeId)
        {
            try
            {
                await _client.Child("users")
                                 .Child(userId)
                                 .Child("favorites")
                                 .Child(recipeId)
                                 .DeleteAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Ratings CRUD operations

        public async Task<double> GetAverageRatingAsync(string recipeId)
        {
            try
            {
                var avgRating = await _client.Child("recipeRatings")
                                             .Child(recipeId)
                                             .Child("avgRating")
                                             .OnceSingleAsync<double>();
                return avgRating;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> GetUserRatingAsync(string userId, string recipeId)
        {
            try
            {
                var rating = await _client.Child("recipeRatings")
                                          .Child(recipeId)
                                          .Child("users")
                                          .Child(userId)
                                          .OnceSingleAsync<int>();
                return rating;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddUserRatingAsync(string userId, string recipeId, int rating)
        {
            try
            {
                var obj = await _client.Child("recipeRatings")
                                          .Child(recipeId)
                                          .OnceSingleAsync<Rating>();

                if (obj == null)
                {
                    obj = new Rating();
                }

                // Business logic for handling ratings
                UpdateUserRating(ref obj, userId, rating);

                await _client.Child("recipeRatings")
                             .Child(recipeId)
                             .PutAsync(obj);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateUserRatingAsync(string userId, string recipeId, int newRating)
        {
            try
            {
                var obj = await _client.Child("recipeRatings")
                                       .Child(recipeId)
                                       .OnceSingleAsync<Rating>();

                // Business logic for handling ratings
                UpdateUserRating(ref obj, userId, newRating);

                await _client.Child("recipeRatings")
                             .Child(recipeId)
                             .PutAsync(obj);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteUserRatingAsync(string userId, string recipeId)
        {
            try
            {
                var obj = await _client.Child("recipeRatings")
                                       .Child(recipeId)
                                       .OnceSingleAsync<Rating>();

                // Delete entire recipe rating node if one rater
                if (obj.users.ContainsKey(userId) && obj.users.Count == 1)
                {
                    await _client.Child("recipeRatings")
                                 .Child(recipeId)
                                 .DeleteAsync();
                }
                // Recalculate values and remove rater if multiple raters
                else if (obj.users.ContainsKey(userId) && obj.users.Count > 1)
                {
                    DeleteUserRating(ref obj, userId);

                    await _client.Child("recipeRatings")
                                 .Child(recipeId)
                                 .PutAsync(obj);
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Private helper methods

        /// <summary>
        /// Adds or updates the recipe rating for the given user.
        /// 3 cases:
        /// 1.) Recipe has no ratings.
        /// 2.) Recipe has ratings but user hasn't rated the recipe before.
        /// 3.) Recipe has ratings and user has already rated the recipe.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="rating"></param>
        private void UpdateUserRating(ref Rating rating, string userId, int newRating)
        {
            if (rating == null)
                throw new NullReferenceException();
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be null or whitespace.");
            if (newRating < 1 || newRating > 5)
                throw new ArgumentException("Rating cannot be less than 1 or greater than 5.");
            if (!string.IsNullOrWhiteSpace(userId) && newRating >= 1 && newRating <= 5)
            {
                // Case 1
                if (rating.numRatings == 0 && rating.users.Count == 0)
                {
                    rating.numRatings = 1;
                }

                // Case 2
                else if (rating.numRatings > 0 && !rating.users.ContainsKey(userId))
                {
                    rating.numRatings++;
                }

                // Case 3
                else if (rating.numRatings > 0 && rating.users.ContainsKey(userId))
                {
                }
                rating.users[userId] = newRating;
                rating.avgRating = (double)rating.users.Sum(x => x.Value) / (double)(rating.numRatings);
            }
        }

        private void DeleteUserRating(ref Rating rating, string userId)
        {
            if (rating == null)
                throw new NullReferenceException();
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be null or whitespace.");
            if (!string.IsNullOrWhiteSpace(userId))
            {

                rating.users.Remove(userId);

                rating.avgRating = (double)rating.users.Sum(x => x.Value) / (double)(rating.users.Count);
                rating.numRatings = rating.users.Count;
            }
        }

        #endregion
    }
}
