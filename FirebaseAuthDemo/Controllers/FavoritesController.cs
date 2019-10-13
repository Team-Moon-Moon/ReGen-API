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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private IDatabaseService _dbClient;
        private IAuthService _authClient;

        public FavoritesController(IDatabaseService dbClient, IAuthService authClient)
        {
            _dbClient = dbClient;
            _authClient = authClient;
        }

        //public FavoritesController(IDatabaseService dbClient)
        //{
        //    _dbClient = dbClient;
        //}

        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            try
            {
                var headerContents = Request.Headers["Authorization"];

                var accessToken = headerContents.ToString().Split(' ')[1];

                var uid = await _authClient.GetUid(accessToken);

                var obj = await _dbClient.GetUserFavoritesAsync(uid);
                return Ok(obj);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPut("{recipeId}")]
        public async Task<IActionResult> AddFavoriteRecipe(string recipeId)
        {
            try
            {
                var headerContents = Request.Headers["Authorization"];

                var accessToken = headerContents.ToString().Split(' ')[1];

                var uid = await _authClient.GetUid(accessToken);

                await _dbClient.AddUserFavoriteAsync(uid, recipeId);

                return Ok();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpDelete("{recipeId}")]
        public async Task<IActionResult> DeleteFavoriteRecipe(string recipeId)
        {
            try
            {
                var headerContents = Request.Headers["Authorization"];

                var accessToken = headerContents.ToString().Split(' ')[1];

                var uid = await _authClient.GetUid(accessToken);

                await _dbClient.DeleteUserFavoriteAsync(uid, recipeId);

                return Ok();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}