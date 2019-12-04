using FirebaseAuthDemo.Models;
using FirebaseAuthDemo.Utils.Custom_Exceptions;
using FirebaseAuthDemo.Utils.Validation.Http_Request_Body_Schemas;
using FirebaseAuthDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IDatabaseService _dbClient;
        private readonly IAuthService _authClient;
        private readonly IValidationService _vClient;

        public ReviewService(IDatabaseService databaseClient, IAuthService authClient, IValidationService validationClient)
        {
            _dbClient = databaseClient;
            _authClient = authClient;
            _vClient = validationClient;
        }

        public async Task<ReviewViewModel> GetUserReviewAsync(string userId, string recipeId)
        {
            try
            {
                var review = await _dbClient.GetUserReviewAsync(userId, recipeId);

                if (review != null)
                {
                    return new ReviewViewModel
                    {
                        Author = review.UserId,
                        Content = review.Content,
                        Timestamp = review.Timestamp,
                        Rating = review.Rating
                    };
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ReviewsPage> GetReviewsPageAsync(string recipeId, string pageStartKey, int pageSize)
        {
            int min = 1, max = 100;
            if (pageSize >= min && pageSize <= max)
            {
                return await _dbClient.GetReviewsPageAsync(recipeId, pageStartKey, pageSize);
            }
            else
            {
                throw new ArgumentException($"Page size must be between {min} and {max}.");
            }
        }

        public async Task AddReviewAsync(string userId, string recipeId, ReviewForm reviewForm)
        {
            try
            {
                Review review =
                    new Review
                    {
                        UserId = userId,
                        RecipeId = recipeId,
                        Content = reviewForm.Content,
                        Rating = reviewForm.Rating,
                        Timestamp = DateTime.Now
                    };

                //int? existingRating = await _dbClient.GetUserRatingAsync(userId, recipeId);
                //if (existingRating == null)
                //{
                //    await _dbClient.AddUserRatingAsync(userId, review.RecipeId, review.Rating);
                //}
                int existingRating = await _dbClient.GetUserRatingAsync(userId, recipeId);
                if (existingRating == 0)
                {
                    await _dbClient.AddUserRatingAsync(userId, review.RecipeId, review.Rating);
                }
                else
                {
                    await _dbClient.UpdateUserRatingAsync(userId, review.RecipeId, review.Rating);
                }

                await _dbClient.AddReviewAsync(review);
            }
            catch (Exception)
            {
                //RollbackAddReview();
                throw;
            }
        }

        public async Task UpdateReview(string userId, string recipeId, ReviewForm reviewForm)
        {
            try
            {
                Review review =
                    new Review
                    {
                        UserId = userId,
                        RecipeId = recipeId,
                        Content = reviewForm.Content,
                        Rating = reviewForm.Rating,
                        Timestamp = DateTime.Now
                    };

                // Update to rating must be performed before update to review. The database will be left in an inconsistent state otherwise.
                await _dbClient.UpdateUserRatingAsync(userId, review.RecipeId, review.Rating);

                await _dbClient.UpdateReviewAsync(review);

            }
            catch (Exception)
            {
                //RollbackUpdateReview();
                throw;
            }
        }

        public async Task DeleteReview(string userId, string recipeId)
        {
            try
            {
                await _dbClient.DeleteReviewAsync(userId, recipeId);
            }
            catch (Exception)
            {
                //RollbackDeleteReview();
                throw;
            }
        }

        #region Not implemented

        private void RollbackAddReview()
        {
        }

        private void RollbackUpdateReview()
        {
        }

        private void RollbackDeleteReview()
        {
        } 

        #endregion
    }
}
