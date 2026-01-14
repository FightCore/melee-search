using MeleeSearch.Models.Search;
using MeleeSearch.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MeleeSearch.APi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string q,
        [FromQuery] string? character = null,
        [FromQuery] string? type = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(new { error = "Query parameter 'q' is required" });
        }

        var context = new SearchContext
        {
            Query = q,
            Character = character,
            PreferredDataEntryType = type
        };

        var results = await _searchService.SearchAsync(context, cancellationToken);
        return Ok(results);
    }
}
