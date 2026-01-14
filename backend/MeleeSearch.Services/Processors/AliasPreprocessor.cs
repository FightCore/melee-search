using System.Text.RegularExpressions;
using MeleeSearch.Data.Context;
using MeleeSearch.Models.Search;
using MeleeSearch.Repositories.Caching;
using MeleeSearch.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeleeSearch.Services.Implementations;

public class AliasPreprocessor : IQueryPreprocessor
{
    private readonly MeleeSearchDbContext _context;
    private readonly ICacheService _cache;
    private const string CacheKey = "query-aliases";

    public AliasPreprocessor(MeleeSearchDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }
    
    public int Priority => 2;
    public async Task<SearchContext> PreprocessQueryAsync(SearchContext searchContext, CancellationToken cancellationToken = default)
    {
        var aliases = await GetAliasesAsync(cancellationToken);
        var processedQuery = searchContext.Query;

        // Sort aliases by term length (longest first) to handle multi-word aliases correctly
        // e.g., "captain falcon" should be processed before "falcon"
        foreach (var alias in aliases.OrderByDescending(a => a.Term.Length))
        {
            // Use word boundary matching for case-insensitive replacement
            var pattern = $@"\b{Regex.Escape(alias.Term)}\b";
            processedQuery = Regex.Replace(
                processedQuery,
                pattern,
                alias.Replacement,
                RegexOptions.IgnoreCase
            );
        }

        searchContext.Query = processedQuery;
        return searchContext;
    }

    private async Task<List<(string Term, string Replacement)>> GetAliasesAsync(CancellationToken cancellationToken)
    {
        // var cachedAliases = await _cache.GetAsync<List<(string, string)>>(CacheKey, cancellationToken);
        // if (cachedAliases != null)
        // {
        //     return cachedAliases;
        // }

        var aliases = await _context.Aliases
            .AsNoTracking()
            .Select(a => new ValueTuple<string, string>(a.Term, a.Replacement))
            .ToListAsync(cancellationToken);

        //await _cache.SetAsync(CacheKey, aliases, TimeSpan.FromHours(1), cancellationToken);

        return aliases;
    }
}