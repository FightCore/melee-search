using System.Data;
using MeleeSearch.Data.Context;
using MeleeSearch.Models.Entities;
using MeleeSearch.Repositories.Caching;
using MeleeSearch.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MeleeSearch.Repositories.Implementations;

public class SearchRepository : ISearchRepository
{
    private readonly IDbConnection _connection;
    private readonly ILogger<SearchRepository> _logger;
    private readonly MeleeSearchDbContext _context;
    private readonly ICacheService _cacheService;

    public SearchRepository(IDbConnection connection, MeleeSearchDbContext context, ILogger<SearchRepository> logger, ICacheService cacheService)
    {
        _connection = connection;
        _context = context;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<List<DataEntry>> GetAllEntries(CancellationToken cancellationToken = default)
    {
        const string cacheKey = "all-data-entries";
        var cachedValue = await _cacheService.GetAsync<List<DataEntry>>(cacheKey, cancellationToken);
        if (cachedValue != null)
        {
            return cachedValue;
        }

        var dataEntries = await _context.DataEntries
            .AsNoTracking()
            .Include(dataEntry => dataEntry.Tags)
            .Include(dataEntry => dataEntry.Characters)
            .ToListAsync(cancellationToken);

        // await _cacheService.SetAsync(cacheKey, dataEntries, TimeSpan.FromHours(1), cancellationToken);

        return dataEntries;
    }
}
