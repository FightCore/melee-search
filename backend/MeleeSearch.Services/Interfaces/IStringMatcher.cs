namespace MeleeSearch.Services.Interfaces;

/// <summary>
/// Generic interface for string matching algorithms used in search scoring.
/// Implementations can use any matching strategy (fuzzy, exact, phonetic, AI-based, etc.)
/// and should work with normalized text (lowercase, no special chars, no spaces).
/// </summary>
public interface IStringMatcher
{
    /// <summary>
    /// Calculates a similarity score between two normalized strings.
    /// The implementation decides the matching strategy.
    /// </summary>
    /// <param name="text1">First normalized string to compare</param>
    /// <param name="text2">Second normalized string to compare</param>
    /// <returns>Similarity score between 0 and 100, where 100 is a perfect match</returns>
    int CalculateMatchScore(string text1, string text2);
}
