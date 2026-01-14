using MeleeSearch.Data.Statics;
using MeleeSearch.Models.Search;
using MeleeSearch.Services.Interfaces;

namespace MeleeSearch.Services.Implementations;

public class CharacterMatcherPreprocessor : IQueryPreprocessor
{
    private const int CharacterMinThreshold = 70;
    public int Priority => int.MaxValue;
    
    private readonly IStringMatcher _matcher;

    public CharacterMatcherPreprocessor(IStringMatcher matcher)
    {
        _matcher = matcher;
    }

    public async Task<SearchContext> PreprocessQueryAsync(SearchContext searchContext, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchContext.Query) || !string.IsNullOrWhiteSpace(searchContext.Character))
        {
            return searchContext;
        }

        var characterSection = string.Empty;
        string? foundCharacter = null;
        int? lastIndex = null;
        var highestScore = int.MinValue;

        var keyWords = searchContext.Query.Split(' ');
        
        for (var i = 0; i < keyWords.Length; i++)
        {
            characterSection += keyWords[i];

            foreach (var character in Characters.MeleeCharacters)
            {
                var distance = _matcher.CalculateMatchScore(character, characterSection);
                if (distance > CharacterMinThreshold && distance > highestScore)
                {
                    highestScore = distance;
                    foundCharacter = character;
                    lastIndex = i;
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(foundCharacter))
        {
            searchContext.Character = foundCharacter;
            searchContext.Query = string.Join(' ', keyWords[(lastIndex!.Value + 1)..]);
        }

        return searchContext;
    }
}