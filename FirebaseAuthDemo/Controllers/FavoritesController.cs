using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAuthDemo.Filters;
using FirebaseAuthDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FirebaseAuthDemo.Controllers
{
    /// <summary>
    /// Action methods for the recipe favoriting system. The user must be authenticated for all methods.
    /// </summary>
    [ServiceFilter(typeof(ValidationFilter))]
    [ServiceFilter(typeof(HttpResponseExceptionFilter))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class FavoritesController : ControllerBase
    {
        private IDatabaseService _dbClient;
        private IAuthService _authClient;

        public FavoritesController(IDatabaseService databaseClient, IAuthService authClient)
        {
            _dbClient = databaseClient;
            _authClient = authClient;
        }

        /// <summary>
        /// [Requires authentication] Returns a collection of the user's favorited recipes.
        /// </summary>
        /// <returns>HTTP status code 200 with a dictionary of recipe IDs and ONLY 'true' values (no 'false' values), or 204 if no favorites found.</returns>
        /// <response code="200">Returns a list of favorited recipe IDs.</response>
        /// <response code="204">The user has no recipes favorited.</response>
        /// <response code="401">The request is missing an authorization header.</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetUserFavorites()
        {
            var authHeaderContents = Request.Headers["Authorization"];
            var accessToken = authHeaderContents.ToString().Split(' ')[1];
            var uid = await _authClient.GetUid(accessToken);

            IDictionary<string, bool> favorites = await _dbClient.GetUserFavoritesAsync(uid);

            if (favorites != null)
                return Ok(favorites);

            return NoContent();
        }

        /// <summary>
        /// [Requires authentication] Adds a recipe to the user's favorites.
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <returns>HTTP status code 204 indicating success.</returns>
        /// <response code="204">The request is successful.</response>
        /// <response code="401">The request is missing an authorization header.</response>
        /// <response code="404">The recipe could not be found.</response>
        [HttpPut("{recipeId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddFavoriteRecipe(string recipeId)
        {
            var authHeaderContents = Request.Headers["Authorization"];
            var accessToken = authHeaderContents.ToString().Split(' ')[1];
            var uid = await _authClient.GetUid(accessToken);

            await _dbClient.AddUserFavoriteAsync(uid, recipeId);

            return NoContent();
        }

        /// <summary>
        /// [Requires authentication] Deletes a recipe from the user's favorites.
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <returns>HTTP status code 204 indicating success.</returns>
        /// <response code="204">The request is successful.</response>
        /// <response code="401">The request is missing an authorization header.</response>
        /// <response code="404">The recipe could not be found.</response>
        [HttpDelete("{recipeId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteFavoriteRecipe(string recipeId)
        {
            var authHeaderContents = Request.Headers["Authorization"];
            var accessToken = authHeaderContents.ToString().Split(' ')[1];
            var uid = await _authClient.GetUid(accessToken);

            await _dbClient.DeleteUserFavoriteAsync(uid, recipeId);

            return Ok();
        }
    }
}