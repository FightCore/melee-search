using System.Text.RegularExpressions;

namespace MeleeSearch.Services.Utilities;

public static partial class TextNormalizer
{
    public static string Normalize(string text)
    {

            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            // Convert to lowercase
            var normalized = text.ToLowerInvariant();

            // Remove special characters except spaces
            normalized = SpecialCharacterRegex().Replace(normalized, "");

            // Replace multiple spaces with single space
            normalized = DetectMultipleSpacesRegex().Replace(normalized, " ");

            return normalized.Trim();
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex DetectMultipleSpacesRegex();
    [GeneratedRegex(@"[^\w\s]")]
    private static partial Regex SpecialCharacterRegex();
}