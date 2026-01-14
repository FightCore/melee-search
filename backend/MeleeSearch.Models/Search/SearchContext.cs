namespace MeleeSearch.Models.Search;

/// <summary>
/// Represents the context for a search operation, including the query and optional filters.
/// </summary>
public class SearchContext
{
    /// <summary>
    /// The search query string.
    /// </summary>
    public required string Query { get; set; }

    /// <summary>
    /// Optional character filter to limit search results to a specific character.
    /// </summary>
    public string? Character { get; set; }

    /// <summary>
    /// Optional preferred data entry type filter (e.g., "frame_data", "character_attribute").
    /// </summary>
    public string? PreferredDataEntryType { get; set; }
}
