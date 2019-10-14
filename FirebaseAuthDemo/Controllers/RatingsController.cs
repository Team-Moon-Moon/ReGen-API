using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAuthDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace FirebaseAuthDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private IDatabaseService _dbClient;
        private IAuthService _authClient;
        private IValidationService _vClient;

        public RatingsController(IDatabaseService databaseClient, IAuthService authClient, IValidationService validationClient)
        {
            _dbClient = databaseClient;
            _authClient = authClient;
            _vClient = validationClient;
        }

        /// <summary>
        /// Returns a recipe's average rating. 
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <returns></returns>
        [HttpGet("{recipeId}")]
        public async Task<IActionResult> GetAverageRating(string recipeId)
        {
            try
            {
                var rating = await _dbClient.GetAverageRatingAsync(recipeId);

                return Ok(rating);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Returns the user's recipe rating, if they rated it. The user must be authenticated.
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{recipeId}")]
        public async Task<IActionResult> GetUserRating(string recipeId)
        {
            try
            {
                var authHeaderContents = Request.Headers["Authorization"];
                var accessToken = authHeaderContents.ToString().Split(' ')[1];
                var uid = await _authClient.GetUid(accessToken);

                var rating = await _dbClient.GetUserRatingAsync(uid, recipeId);

                return Ok(rating);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [Authorize]
        [HttpPut("{recipeId}")]
        public async Task<IActionResult> AddUserRating(string recipeId, [FromBody]JObject data)
        {
            try
            {
                var authHeaderContents = Request.Headers["Authorization"];
                var accessToken = authHeaderContents.ToString().Split(' ')[1];
                var uid = await _authClient.GetUid(accessToken);

                _vClient.ValidateRatingPutRequest(data);

                await _dbClient.AddUserRatingAsync(uid, recipeId, data["Rating"].ToObject<int>());

                return Ok();
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}