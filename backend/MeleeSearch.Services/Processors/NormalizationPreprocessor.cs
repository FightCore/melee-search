using MeleeSearch.Models.Search;
using MeleeSearch.Services.Interfaces;
using MeleeSearch.Services.Utilities;

namespace MeleeSearch.Services.Implementations;

public class NormalizationPreprocessor : IQueryPreprocessor
{
    public int Priority => 1;

    public Task<SearchContext> PreprocessQueryAsync(SearchContext context,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(context.Query))
        {
            return Task.FromResult(context);
        }
        
        context.Query = TextNormalizer.Normalize(context.Query);
        return Task.FromResult(context);
    }
}
