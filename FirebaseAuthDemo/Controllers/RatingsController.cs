using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAuthDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FirebaseAuthDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private IDatabaseService _dbClient;
        private IAuthService _authClient;

        public RatingsController(IDatabaseService dbClient, IAuthService authClient)
        {
            _dbClient = dbClient;
            _authClient = authClient;
        }

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
        [HttpGet("monkey")]
        public async Task<IActionResult> DoAlot(string recipeId)
        {
            try
            {
                var authHeaderContents = Request.Headers["Authorization"];

                var accessToken = authHeaderContents.ToString().Split(' ')[1];

                var uid = await _authClient.GetUid(accessToken);

                recipeId = "monkey";

                await _dbClient.AddUserRatingAsync(uid, recipeId, 15);

                var rating = await _dbClient.GetUserRatingAsync(uid, recipeId);

                await _dbClient.DeleteUserRatingAsync(uid, recipeId);

                return Ok();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}