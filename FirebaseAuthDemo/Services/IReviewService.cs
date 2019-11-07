using FirebaseAuthDemo.Models;
using FirebaseAuthDemo.Utils.Validation.Http_Request_Body_Schemas;
using FirebaseAuthDemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Services
{
    public interface IReviewService
    {
        Task<ReviewViewModel> GetUserReviewAsync(string userId, string recipeId);

        Task<ReviewsPage> GetReviewsPageAsync(string recipeId, string pageStartKey, int pageSize);

        Task AddReviewAsync(string userId, string recipeId, ReviewForm reviewForm);

        Task UpdateReview(string userId, string recipeId, ReviewForm reviewForm);

        Task DeleteReview(string userId, string recipeId);
    }
}
