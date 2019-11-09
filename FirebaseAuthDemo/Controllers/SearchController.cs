using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAuthDemo.Filters;
using FirebaseAuthDemo.Services;
using FirebaseAuthDemo.Utils.Custom_Exceptions;
using FirebaseAuthDemo.Utils.Validation.Http_Request_Body_Schemas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FirebaseAuthDemo.Controllers
{
    [ServiceFilter(typeof(ValidationFilter))]
    [ServiceFilter(typeof(HttpResponseExceptionFilter))]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class SearchController : ControllerBase
    {
        private ISearchService _searchClient;

        public SearchController(ISearchService searchClient)
        {
            _searchClient = searchClient;
        }

        /// <summary>
        /// Returns a list of recipes matching the query. An unfiltered search (no preferences) is performed.
        /// </summary>
        /// <param name="q">The query term(s).</param>
        /// <returns>A list of Recipe objects.</returns>
        /// <response code="200">Returns a list of Recipe objects.</response>
        /// <response code="204">No recipes found.</response>
        /// <response code="400">The query parameter is missing, or the request body is invalid.</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SearchRecipes([FromQuery]string q = "")
        {
            var results = await _searchClient.SearchUnfilteredAsync(q);

            if (results != null)
                return Ok(results);

            return NoContent();
        }

        /// <summary>
        /// Returns a list of recipes matching the query. A filtered search (with preferences) is performed.
        /// </summary>
        /// <remarks>For unfiltered searches, include an JSON empty request body.</remarks>
        /// <param name="q">The query term(s).</param>
        /// <param name="searchForm">The request body as JSON.</param>
        /// <returns>A list of Recipe objects.</returns>
        /// <response code="200">Returns a list of Recipe objects.</response>
        /// <response code="204">No recipes found.</response>
        /// <response code="400">The query parameter is missing, or the request body is invalid.</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SearchRecipes([FromQuery]string q = "", [FromBody]SearchForm searchForm = null)
        {
            var results = await _searchClient.SearchFilteredAsync(q, searchForm?.IncludeTags, searchForm?.ExcludeTags);

            if (results != null)
                return Ok(results);

            return NoContent();
        }
    }
}