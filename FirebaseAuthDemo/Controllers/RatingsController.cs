using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAuthDemo.Filters;
using FirebaseAuthDemo.Services;
using FirebaseAuthDemo.Validation;
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
    public class RatingsController : ControllerBase
    {
        private readonly IDatabaseService _dbClient;
        private readonly IAuthService _authClient;

        public RatingsController(IDatabaseService databaseClient, IAuthService authClient)
        {
            _dbClient = databaseClient;
            _authClient = authClient;
        }

        /// <summary>
        /// Returns a recipe's average rating. 
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <returns>HTTP status code 200 with the average rating as a double.</returns>
        /// <response code="200">Returns the recipe's average rating</response>
        /// <response code="204">The recipe has no ratings.</response>
        /// <response code="404">The recipe could not be found.</response>
        [HttpGet("{recipeId}/average")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAverageRating(string recipeId)
        {
            double? rating = await _dbClient.GetAverageRatingAsync(recipeId);

            if (rating != null)
                return Ok(rating);

            return NoContent();
        }

        /// <summary>
        /// [Requires authentication] Returns the user's recipe rating, if they rated it.
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <returns>HTTP status code 200 with the user's rating as an integer.</returns>
        /// <response code="200">Returns the user's rating for the recipe.</response>
        /// <response code="204">The user hasn't rated the recipe.</response>
        /// <response code="401">The request is missing an authorization header.</response>
        /// <response code="404">The recipe could not be found.</response>
        [Authorize]
        [HttpGet("{recipeId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserRating(string recipeId)
        {
            var authHeaderContents = Request.Headers["Authorization"];
            var accessToken = authHeaderContents.ToString().Split(' ')[1];
            var uid = await _authClient.GetUid(accessToken);

            int? rating = await _dbClient.GetUserRatingAsync(uid, recipeId);

            if (rating != null)
                return Ok(rating);

            return NoContent();
        }

        /// <summary>
        /// [Requires authentication] Creates a new recipe rating for the user.
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <param name="form">The rating as a JSON body.</param>
        /// <returns>HTTP status code 200 with no content.</returns>
        /// <response code="204">The request is successful.</response>
        /// <response code="400">The request body is invalid.</response>
        /// <response code="401">The request is missing an authorization header.</response>
        /// <response code="404">The recipe could not be found.</response>
        /// <response code="409">A rating for this user already exists.</response>
        [Authorize]
        [HttpPut("{recipeId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> AddUserRating(string recipeId, [FromBody]UserRatingForm form)
        {
            var authHeaderContents = Request.Headers["Authorization"];
            var accessToken = authHeaderContents.ToString().Split(' ')[1];
            var uid = await _authClient.GetUid(accessToken);

            // Nullable ints are ridiculous.
            await _dbClient.AddUserRatingAsync(uid, recipeId, form.Rating ?? int.MaxValue);

            return NoContent();
        }

        /// <summary>
        /// [Requires authentication] Updates the user's existing recipe rating.
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <param name="form">The new rating as a JSON body</param>
        /// <returns>HTTP status code 200 with no content.</returns>
        /// <response code="204">The request is successful.</response>
        /// <response code="400">The request body is invalid.</response>
        /// <response code="401">The request is missing an authorization header.</response>
        /// <response code="404">The recipe could not be found.</response>
        [Authorize]
        [HttpPost("{recipeId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUserRating(string recipeId, [FromBody]UserRatingForm form)
        {
            var authHeaderContents = Request.Headers["Authorization"];
            var accessToken = authHeaderContents.ToString().Split(' ')[1];
            var uid = await _authClient.GetUid(accessToken);

            // Nullable ints are ridiculous. This will send a max int to the database (bad).
            await _dbClient.UpdateUserRatingAsync(uid, recipeId, form.Rating ?? int.MaxValue);

            return NoContent();
        }

        /// <summary>
        /// Unimplemented method. Ratings should not be deleted because submitting/retrieving a review requires a rating.
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <returns>HTTP status code 403 indicating access denied.</returns>
        [HttpDelete("{recipeId}")]
        private async Task<IActionResult> DeleteUserRating(string recipeId)
        {
            return Forbid();
        }
    }
}