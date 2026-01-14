using MeleeSearch.Models.Entities;

namespace MeleeSearch.Repositories.Interfaces;

public interface ISearchRepository
{
    Task<List<DataEntry>> GetAllEntries(CancellationToken cancellationToken = default);
}
