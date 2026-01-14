using MeleeSearch.Models.Search;

namespace MeleeSearch.Services.Interfaces;

public interface IQueryPreprocessor
{
    /// <summary>
    /// Priority that determines the order that the processors are ran at
    /// </summary>
    int Priority { get; }

    Task<SearchContext> PreprocessQueryAsync(SearchContext searchContext, CancellationToken cancellationToken = default);
}
