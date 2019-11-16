using Firebase.Database;
using Firebase.Database.Query;
using FirebaseAuthDemo.Models;
using FirebaseAuthDemo.Utils.Custom_Exceptions;
using FirebaseAuthDemo.Utils.Validation.Http_Request_Body_Schemas;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
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


                if (result != null)
                {
                    result.Key = recipeId;
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task AddRecipeAsync(Recipe recipe)
        {
            try
            {
                await _client.Child("recipes")
                             .PostAsync<Recipe>(recipe);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateRecipeAsync(Recipe recipe)
        {
            try
            {
                await _client.Child("recipes")
                             .Child(recipe.Key)
                             .PatchAsync<Recipe>(recipe);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteRecipeAsync(string recipeId)
        {
            try
            {
                var obj = _client.Child("recipes")
                                 .Child(recipeId)
                                 .OnceSingleAsync<Recipe>();

                if (obj == null)
                {
                    throw new NoResultsFoundException("Recipe not found for ID: " + recipeId);
                }
                    

                await _client.Child("recipes")
                             .Child(recipeId)
                             .DeleteAsync();

                await DeleteAllRatingsAsync(recipeId);
                await DeleteAllReviewsAsync(recipeId);
            }
            catch (Exception)
            {
                throw;
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
                
                if (result == null || result.Count == 0)
                {
                    return null;
                }

                var favorites = result.ToDictionary(o => (o.Key),
                                                    o => (o.Object));

                if (favorites.Count == 0)
                {
                    return null;
                }

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
                ////This code could bug out if recipes are deleted.
                //var result = await GetRecipeAsync(recipeId);

                //if (result == null)
                //{
                //    throw new NoResultsFoundException("Recipe not found for ID: " + recipeId);
                //}
        
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
            catch (FirebaseException) // No ratings
            {
                return 0;
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
            catch (FirebaseException) // No ratings
            {
                return 0;
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
                var r = await GetRecipeAsync(recipeId);
                if (r == null)
                {
                    throw new NoResultsFoundException("No recipe found for ID: " + recipeId);
                }

                var obj = await _client.Child("recipeRatings")
                                       .Child(recipeId)
                                       .OnceSingleAsync<Rating>();

                if (obj == null)
                {
                    obj = new Rating();
                }
                else if (obj.users.ContainsKey(userId))
                {
                    throw new ResourceAlreadyExistsException($"Rating for User ID {userId} already exists on recipe ID {recipeId}");
                }

                // Business logic for handling ratings
                UpdateUserRating(userId, ref obj, rating);

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
                var r = await GetRecipeAsync(recipeId);
                if (r == null)
                {
                    throw new NoResultsFoundException("No recipe found for ID: " + recipeId);
                }

                var obj = await _client.Child("recipeRatings")
                                       .Child(recipeId)
                                       .OnceSingleAsync<Rating>();

                if (obj == null || !obj.users.ContainsKey(userId))
                {
                    throw new NoResultsFoundException($"No rating found for user ID {userId} on recipe ID {recipeId}.");
                }

                // Business logic for handling ratings
                UpdateUserRating(userId, ref obj, newRating);

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
                var r = await GetRecipeAsync(recipeId);
                if (r == null)
                {
                    throw new NoResultsFoundException("No recipe found for ID: " + recipeId);
                }

                var obj = await _client.Child("recipeRatings")
                                       .Child(recipeId)
                                       .OnceSingleAsync<Rating>();

                if (obj == null)
                {
                    throw new NoResultsFoundException($"No rating found for user ID {userId} on recipe ID {recipeId}.");
                }

                // Delete entire recipe rating node if one rater
                if (obj.users.ContainsKey(userId) && obj.users.Count == 1)
                {
                    await DeleteAllRatingsAsync(recipeId);
                }
                // Recalculate values and remove rater if multiple raters
                else if (obj.users.ContainsKey(userId) && obj.users.Count > 1)
                {
                    DeleteUserRating(userId, ref obj);

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

        private async Task DeleteAllRatingsAsync(string recipeId)
        {
            try
            {
                await _client.Child("recipeRatings")
                             .Child(recipeId)
                             .DeleteAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Reviews CRUD operations

        public async Task<Review> GetUserReviewAsync(string userId, string recipeId)
        {
            try
            {
                var reviewJson = await _client.Child("reviews")
                                              .Child(recipeId)
                                              .OrderBy("UserId")
                                              .EqualTo(userId)
                                              .OnceSingleAsync<Dictionary<string, Dictionary<string, dynamic>>>();
                
                if (reviewJson == null || !reviewJson.Any())
                {
                    //throw new NoResultsFoundException($"No review found for user ID: {userId} on recipe ID: {recipeId}.");
                    return null;
                }

                var dict = reviewJson.First().Value;

                var rating = await GetUserRatingAsync(userId, recipeId);

                var review =
                    new Review
                    {
                        ReviewId = reviewJson.First().Key,
                        RecipeId = recipeId,
                        UserId = dict["UserId"],
                        Content = dict["Content"],
                        Timestamp = dict["Timestamp"],
                        Rating = rating
                    };

                return review;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ReviewsPage> GetReviewsPageAsync(string recipeId, string pageStartKey, int pageSize)
        {
            try
            {
                // To-do: config options for index names and page sizes
                IReadOnlyCollection<FirebaseObject<Review>> results;

                if (!String.IsNullOrWhiteSpace(pageStartKey))
                {
                    results = await _client.Child("reviews")
                                           .Child(recipeId)
                                           .OrderByKey()
                                           .EndAt(pageStartKey)
                                           .LimitToLast(pageSize + 1)
                                           .OnceAsync<Review>();
                }
                else
                {
                    results = await _client.Child("reviews")
                                           .Child(recipeId)
                                           .OrderByKey()
                                           .LimitToLast(pageSize + 1)
                                           .OnceAsync<Review>();
                }

                if (results.Count < 1)
                {
                    throw new NoResultsFoundException("No reviews found for recipe ID: " + recipeId);
                }


                //var ratings = results.Select(async o => 
                //    new KeyValuePair<string, int>
                //    (o.Object.UserId, 
                //    await GetUserRatingAsync(o.Object.UserId, recipeId))
                //);

                // If the number of results is less than the requested page size, don't generate a "start key"
                var page = results.Count < pageSize ? results : results.Skip(1);

                var ratings = new Dictionary<string, int>();
                foreach(var r in page)
                {
                    var rating = await GetUserRatingAsync(r.Object.UserId, recipeId);
                    ratings.Add(r.Object.UserId, rating);
                }

                var reviews = from pair in page
                              select new Review
                              {
                                  RecipeId = recipeId,
                                  ReviewId = pair.Key,
                                  UserId = pair.Object.UserId,
                                  Content = pair.Object.Content,
                                  Timestamp = pair.Object.Timestamp,
                                  Rating = ratings[pair.Object.UserId]
                              };

                var trimmedPage = reviews.Reverse();
                //trimmedPage.RemoveAt(trimmedPage.Count - 1);

                var reviewPage =
                    new ReviewsPage
                    {
                        NextKey = results.Count < pageSize ? null : results.First().Key,
                        Reviews = trimmedPage
                    };

                return reviewPage;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddReviewAsync(Review review)
        {
            try
            {
                var recipe = await GetRecipeAsync(review.RecipeId);

                if (recipe == null)
                {
                    throw new NoResultsFoundException("No recipe found for ID: " + review.RecipeId);
                }

                var existingReview = await GetUserReviewAsync(review.UserId, review.RecipeId);

                if (existingReview != null)
                {
                    throw new ResourceAlreadyExistsException("User has already submitted a review for recipe ID: " + review.RecipeId);
                }

                await _client.Child("reviews")
                             .Child(review.RecipeId)
                             .PostAsync<Review>(review);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public async Task UpdateReviewAsync(Review review)
        {
            try
            {
                await DeleteReviewAsync(review.UserId, review.RecipeId);
                await AddReviewAsync(review);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Internal method
        private async Task DeleteAllReviewsAsync(string recipeId)
        {
            try
            {
                await _client.Child("reviews")
                             .Child(recipeId)
                             .DeleteAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public async Task DeleteReviewAsync(string userId, string recipeId)
        {
            try
            {
                var reviewJson = await _client.Child("reviews")
                                              .Child(recipeId)
                                              .OrderBy("UserId")
                                              .EqualTo(userId)
                                              .OnceSingleAsync<Dictionary<string, Dictionary<string, dynamic>>>();

                if (reviewJson == null || !reviewJson.Any())
                {
                    //var v = await _client.Child("reviews")
                    //                          .Child(recipeId)
                    //                          .OrderBy("UserId")
                    //                          .EqualTo(userId).BuildUrlAsync();
                    throw new NoResultsFoundException($"No review found for user ID: {userId} on recipe ID: {recipeId}.");
                }

                var reviewKey = reviewJson.First().Key;

                await _client.Child("reviews")
                             .Child(recipeId)
                             .Child(reviewKey)
                             .DeleteAsync();
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
        private void UpdateUserRating(string userId, ref Rating rating, int newRating)
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

        private void DeleteUserRating(string userId, ref Rating rating)
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
