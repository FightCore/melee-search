using System.Text.RegularExpressions;
using MeleeSearch.Models.Entities;
using MeleeSearch.Models.Search;
using MeleeSearch.Services.Interfaces;

namespace MeleeSearch.Services.Utilities;

public class SearchScorer
{
    private readonly IStringMatcher _matcher;

    public SearchScorer(IStringMatcher matcher)
    {
        _matcher = matcher;
    }
    private const int TitleMaxScore = 100;
    private const int TitleMinThreshold = 60;

    private const int TagMaxScore = 40;
    private const int TagMinThreshold = 70;

    private const int DataMaxScore = 15;
    private const int DataMinThreshold = 60;

    /// <summary>
    /// Calculates relevance score for a data entry based on query matches and search context filters.
    /// Uses pluggable string matching algorithms to accommodate human input variations.
    /// Higher scores indicate better matches and appear first in search results.
    /// </summary>
    /// <param name="entry">The data entry to score</param>
    /// <param name="context">The search context containing query and optional filters</param>
    /// <returns>Tuple containing the calculated score and the entry</returns>
    /// <remarks>
    /// Scoring breakdown (max scores):
    /// - Title matching: 0-100 points (scaled by similarity percentage)
    /// - Tag matching: 0-40 points per tag (scaled by similarity percentage, accumulates)
    /// - JSONB data matching: 0-15 points (scaled by similarity percentage)
    ///
    /// Minimum thresholds to score:
    /// - Title: 60% similarity required
    /// - Tag: 70% similarity required
    /// - Data: 60% similarity required
    ///
    /// Context filters (returns 0 if not matched):
    /// - Character filter: Only include entries for specified character
    /// - PreferredDataEntryType filter: Only include entries of specified type
    /// </remarks>
    public (int score, DataEntry entry) ScoreEntry(DataEntry entry, SearchContext context)
    {
        if (string.IsNullOrWhiteSpace(context.Query))
        {
            return (0, entry);
        }

        // If a character is present, filter out anything that doesn't have that character
        if (!string.IsNullOrWhiteSpace(context.Character) && entry.Characters.All(character => character.Name != context.Character))
        {
            return (0, entry);
        }

        // Apply preferred data entry type filter from context
        if (!string.IsNullOrWhiteSpace(context.PreferredDataEntryType))
        {
            var entryType = GetEntryType(entry);
            var normalizedEntryType = NormalizeText(entryType);
            var normalizedPreferredType = NormalizeText(context.PreferredDataEntryType);

            if (normalizedEntryType != normalizedPreferredType)
            {
                return (0, entry);
            }
        }
        
        var score = 0;
        // Score based on query content
        score += ScoreTitle(entry.Title, context.Query);
        score += ScoreTags(entry.Tags, context.Query);
        score += ScoreData(entry.Data, context.Query);

        return (score, entry);
    }

    private string GetEntryType(DataEntry entry)
    {
        return entry switch
        {
            FrameData => "frame_data",
            CharacterAttribute => "character_attribute",
            _ => "unknown"
        };
    }

    private int ScoreTitle(string title, string query)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return 0;
        }

        var normalizedTitle = NormalizeText(title);
        var matchScore = _matcher.CalculateMatchScore(normalizedTitle, query);

        if (matchScore < TitleMinThreshold)
        {
            return 0;
        }

        return (int)Math.Round(TitleMaxScore * (matchScore / 100.0));
    }

    private int ScoreTags(ICollection<Tag> tags, string query)
    {
        var totalScore = 0;

        foreach (var tag in tags)
        {
            if (string.IsNullOrWhiteSpace(tag.Name))
            {
                continue;
            }

            var normalizedTag = NormalizeText(tag.Name);
            var matchScore = _matcher.CalculateMatchScore(normalizedTag, query);

            if (matchScore >= TagMinThreshold)
            {
                totalScore += (int)Math.Round(TagMaxScore * (matchScore / 100.0));
            }
        }

        return totalScore;
    }

    private int ScoreData(string data, string query)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            return 0;
        }

        var normalizedData = NormalizeText(data);
        var matchScore = _matcher.CalculateMatchScore(normalizedData, query);

        if (matchScore < DataMinThreshold)
        {
            return 0;
        }

        return (int)Math.Round(DataMaxScore * (matchScore / 100.0));
    }
    
    private static string NormalizeText(string text)
    {
        return TextNormalizer.Normalize(text);
    }
}
