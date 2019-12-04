using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAuthDemo.Filters;
using FirebaseAuthDemo.Models;
using FirebaseAuthDemo.Services;
using FirebaseAuthDemo.Utils.Custom_Exceptions;
using FirebaseAuthDemo.Utils.Validation;
using FirebaseAuthDemo.Utils.Validation.Http_Request_Body_Schemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace FirebaseAuthDemo.Controllers
{
    [ServiceFilter(typeof(ValidationFilter))]
    [ServiceFilter(typeof(HttpResponseExceptionFilter))]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewClient;
        private readonly IAuthService _authClient;

        public ReviewsController(IReviewService reviewClient, IAuthService authClient)
        {
            _reviewClient = reviewClient;
            _authClient = authClient;
        }



        /// <summary>
        /// [Requires authentication] Returns the user's review for a recipe.
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <returns>The user's personal review for a recipe.</returns>
        /// <response code="200">Returns the user's review for a recipe.</response>
        /// <response code="204">No review found for the user on the queried recipe.</response>
        /// <response code="400">One or more query parameter(s) are invalid.</response>
        /// <response code="404">No recipe found.</response>
        [Authorize]
        [HttpGet("{recipeId}/self")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPersonalReview(string recipeId)
        {
            var authHeaderContents = Request.Headers["Authorization"];
            var accessToken = authHeaderContents.ToString().Split(' ')[1];
            var uid = await _authClient.GetUid(accessToken);

            var review = await _reviewClient.GetUserReviewAsync(uid, recipeId);
            if (review != null)
            {
                return Ok(review);
            }

            return NoContent();
        }

        /// <summary>
        /// Returns a single page of reviews for a recipe.
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <param name="start">The key to start at.</param>
        /// <param name="size">The number of reviews to return. (min: 1, max: 100, default: 5)</param>
        /// <returns>A page of reviews.</returns>
        /// <response code="200">Returns a list of reviews for a recipe.</response>
        /// <response code="204">No reviews found for the recipe.</response>
        /// <response code="400">One or more query parameter(s) are invalid.</response>
        /// <response code="404">No recipe found.</response>
        [HttpGet("{recipeId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetReviews(string recipeId, [FromQuery]string start = "", [FromQuery]int size = 5)
        {
            var results = await _reviewClient.GetReviewsPageAsync(recipeId, start, size);

            if (results != null)
                return Ok(results);

            return NoContent();
        }

        /// <summary>
        /// [Requires authentication] Adds the user's review for a recipe to ReGen.
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <param name="review">The formatted request body as JSON.</param>
        /// <returns>Http status code 204 indicating success.</returns>
        /// <response code="204">The request is successful.</response>
        /// <response code="400">The request body is invalid.</response>
        /// <response code="401">The request is missing an authorization header.</response>
        /// <response code="404">The recipe could not be found.</response>
        /// <response code="409">A review for this user already exists.</response>
        [Authorize]
        [HttpPut("{recipeId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> CreateReview(string recipeId, [FromBody]ReviewForm review)
        {
            var authHeaderContents = Request.Headers["Authorization"];
            var accessToken = authHeaderContents.ToString().Split(' ')[1];
            var uid = await _authClient.GetUid(accessToken);

            await _reviewClient.AddReviewAsync(uid, recipeId, review);

            return NoContent();
        }

        /// <summary>
        /// [Requires authentication] Updates the user's review for a recipe in ReGen.
        /// </summary>
        /// <param name="review">The formatted request body as JSON.</param>
        /// <returns>Http status code 204 indicating success.</returns>
        /// <response code="204">The request is successful.</response>
        /// <response code="400">The request body is invalid.</response>
        /// <response code="401">The request is missing an authorization header.</response>
        /// <response code="404">The recipe could not be found, or a review for this user could not be found.</response>
        [Authorize]
        [HttpPost("{recipeId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateReview(string recipeId, [FromBody]ReviewForm review)
        {
            var authHeaderContents = Request.Headers["Authorization"];
            var accessToken = authHeaderContents.ToString().Split(' ')[1];
            var uid = await _authClient.GetUid(accessToken);

            await _reviewClient.UpdateReview(uid, recipeId, review);

            return NoContent();
        }

        /// <summary>
        /// [Requires authentication] Deletes the user's review for a recipe in ReGen.
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <returns>Http status code 204 indicating success.</returns>
        /// <response code="204">The request is successful.</response>
        /// <response code="401">The request is missing an authorization header.</response>
        /// <response code="404">The recipe could not be found, or a review for this user could not be found.</response>
        [Authorize]
        [HttpDelete("{recipeId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteReview(string recipeId)
        {
            var authHeaderContents = Request.Headers["Authorization"];
            var accessToken = authHeaderContents.ToString().Split(' ')[1];
            var uid = await _authClient.GetUid(accessToken);

            await _reviewClient.DeleteReview(uid, recipeId);

            return NoContent();
        }
    }
}
