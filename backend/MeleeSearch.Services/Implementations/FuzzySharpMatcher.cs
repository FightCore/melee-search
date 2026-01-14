using FuzzySharp;
using MeleeSearch.Services.Interfaces;

namespace MeleeSearch.Services.Implementations;

public class FuzzySharpMatcher : IStringMatcher
{
    public int CalculateMatchScore(string text1, string text2)
    {
        if (string.IsNullOrWhiteSpace(text1) || string.IsNullOrWhiteSpace(text2))
        {
            return 0;
        }

        // Use multiple FuzzySharp algorithms and return the best score
        // This handles various matching scenarios:
        // - Ratio: Overall similarity
        // - PartialRatio: Best substring match (good for partial matches)
        // - TokenSetRatio: Ignores word order (good for multi-word queries)
        var tokenSetScore = Fuzz.TokenSetRatio(text1, text2);
        var partialScore = Fuzz.PartialRatio(text1, text2);
        var ratioScore = Fuzz.Ratio(text1, text2);

        return Math.Max(tokenSetScore, Math.Max(partialScore, ratioScore));
    }
}
