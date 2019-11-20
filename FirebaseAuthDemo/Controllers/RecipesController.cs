using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAuthDemo.Filters;
using FirebaseAuthDemo.Models;
using FirebaseAuthDemo.Services;
using FirebaseAuthDemo.Utils.Custom_Exceptions;
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
    public class RecipesController : ControllerBase
    {
        private IAuthService _authClient;
        private IRecipeService _recipeClient;

        public RecipesController(IAuthService authClient, IRecipeService recipeClient)
        {
            _authClient = authClient;
            _recipeClient = recipeClient;
        }

        /// <summary>
        /// Finds a recipe by ID.
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <returns>Http status code 200 with a Recipe object, or 204 if none found.</returns>
        /// <response code="200">Returns a Recipe object.</response>
        /// <response code="204">No Recipe found.</response>
        [HttpGet("{recipeId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetRecipe(string recipeId)
        {
            Recipe recipe = await _recipeClient.GetRecipeAsync(recipeId);

            if (recipe != null)
                return Ok(recipe);

           return NoContent();
        }

        /// <summary>
        /// [Requires authentication] Adds a recipe to ReGen.
        /// </summary>
        /// <param name="recipeForm">The formatted request body as JSON.</param>
        /// <returns>Http status code 200 indicating success, along with the newly-created recipe.</returns>
        /// <response code="200">Returns the newly-created recipe.</response>
        /// <response code="400">The request body is invalid.</response>
        /// <response code="401">The request is missing an authorization header.</response>
        [Authorize]
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> AddRecipe([FromBody]RecipeForm recipeForm)
        {
            var authHeaderContents = Request.Headers["Authorization"];
            var accessToken = authHeaderContents.ToString().Split(' ')[1];
            var uid = await _authClient.GetUid(accessToken);
            
            var recipe = await _recipeClient.CreateRecipeAsync(uid, recipeForm);

            return Ok(recipe);
        }

        /// <summary>
        /// [Requires authentication] Updates the user's recipe by ID.
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <param name="recipeForm">The formatted request body as JSON.</param>
        /// <returns>Http status code 204 indicating success.</returns>
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
        public async Task<IActionResult> UpdateRecipe(string recipeId, [FromBody]RecipeForm recipeForm)
        {
            var authHeaderContents = Request.Headers["Authorization"];
            var accessToken = authHeaderContents.ToString().Split(' ')[1];
            var uid = await _authClient.GetUid(accessToken);

            await _recipeClient.UpdateRecipeAsync(uid, recipeId, recipeForm);

            return NoContent();
        }

        // To-do: This method should be admin-only.        
        /// <summary>
        /// [Requires authentication] Deletes the user's recipe by ID.
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <returns>Http status code 204 indicating success.</returns>
        /// <response code="204">The request is successful.</response>
        /// <response code="401">The request is missing an authorization header, or the user does not own the recipe.</response>
        /// <response code="404">The recipe could not be found.</response>
        [Authorize]
        [HttpDelete("{recipeId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteRecipe(string recipeId)
        {
            var authHeaderContents = Request.Headers["Authorization"];
            var accessToken = authHeaderContents.ToString().Split(' ')[1];
            var uid = await _authClient.GetUid(accessToken);

            await _recipeClient.DeleteRecipeAsync(uid, recipeId);

            return NoContent();
        }
    }
}