using MeleeSearch.Models.Search;
using MeleeSearch.Repositories.DTOs;

namespace MeleeSearch.Services.Interfaces;

public interface ISearchService
{
    Task<IEnumerable<SearchCardDto>> SearchAsync(SearchContext context, CancellationToken cancellationToken = default);
}
