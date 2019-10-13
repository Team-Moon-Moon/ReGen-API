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
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private IDatabaseService _db;

        public RecipesController(IDatabaseService dbService)
        {
            _db = dbService;
        }

        [HttpGet("{recipeId}")]
        public async Task<IActionResult> GetRecipe(string recipeId)
        {
            try
            {
                var recipe = await _db.GetRecipeAsync(recipeId);

                return Ok(recipe);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}