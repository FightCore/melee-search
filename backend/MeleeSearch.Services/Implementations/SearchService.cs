using MeleeSearch.Models.Entities;
using MeleeSearch.Models.Search;
using MeleeSearch.Repositories.Caching;
using MeleeSearch.Repositories.DTOs;
using MeleeSearch.Repositories.Interfaces;
using MeleeSearch.Services.Interfaces;
using MeleeSearch.Services.Mappers;
using MeleeSearch.Services.Utilities;

namespace MeleeSearch.Services.Implementations;

public class SearchService : ISearchService
{
    private readonly ISearchRepository _repository;
    private readonly ICacheService _cache;
    private readonly ICardMapperFactory _mapperFactory;
    private readonly IEnumerable<IQueryPreprocessor> _queryPreprocessors;
    private readonly SearchScorer _scorer;
    private const string CacheKeyPrefix = "search";
    private const int MaxItemsPerSearch = 5;

    public SearchService(ISearchRepository repository, ICacheService cache, ICardMapperFactory mapperFactory, IEnumerable<IQueryPreprocessor> queryPreprocessors, SearchScorer scorer)
    {
        _repository = repository;
        _cache = cache;
        _mapperFactory = mapperFactory;
        _queryPreprocessors = queryPreprocessors;
        _scorer = scorer;
    }

    public async Task<IEnumerable<SearchCardDto>> SearchAsync(SearchContext context, CancellationToken cancellationToken = default)
    {
        // Run the preprocessors and replace the context with the new version each time.
        foreach (var processor in _queryPreprocessors.OrderBy(processor => processor.Priority))
        {
            context = await processor.PreprocessQueryAsync(context, cancellationToken);
        }
        

        // Create cache key that includes all context parameters
        var cacheKey = BuildCacheKey(context.Query, context.Character, context.PreferredDataEntryType);

        var cachedResults = await _cache.GetAsync<List<SearchCardDto>>(cacheKey, cancellationToken);
        if (cachedResults != null)
        {
            return cachedResults;
        }

        var entries = await _repository.GetAllEntries(cancellationToken);

        var scoredResults = entries
            .Select(entry => _scorer.ScoreEntry(entry, context))
            .Where(result => result.score > 0)
            .OrderByDescending(result => result.score)
            .Take(MaxItemsPerSearch)
            .Select(result => MapToCard(result.entry))
            .ToList();

        await _cache.SetAsync(cacheKey, scoredResults, TimeSpan.FromMinutes(15), cancellationToken);

        return scoredResults;
    }

    private SearchCardDto MapToCard(DataEntry entry)
    {
        var mapper = _mapperFactory.GetMapper(entry);
        return mapper.MapToCard(entry);
    }

    private static string BuildCacheKey(string query, string? character, string? type)
    {
        var key = $"{CacheKeyPrefix}:{query.ToLowerInvariant()}";

        if (!string.IsNullOrWhiteSpace(character))
        {
            key += $":char:{character.ToLowerInvariant()}";
        }

        if (!string.IsNullOrWhiteSpace(type))
        {
            key += $":type:{type.ToLowerInvariant()}";
        }

        return key;
    }
}
