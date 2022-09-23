using ElasticSearch;
using ElasticSearch.Dtos;
using ElasticSearch.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Search.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ElasticSearchService _elasticSearchService;
        public SearchController(ElasticSearchService elasticSearchService)
        {
            _elasticSearchService = elasticSearchService;
        }


        [HttpGet]
        [Route("AutoComplete")]
        public async Task<IEnumerable<MovieSuggest>> AutoComplete(string keyword) =>
             await _elasticSearchService.AutoComplete(keyword);

        #region CreateMockData
        [HttpPost]
        [Route("CreateMockData")]
        public async Task<IActionResult> CreateMockData()
        {
            var result = await _elasticSearchService.InsertMockData();
            return Ok(result);
        }
        #endregion
    }
}
