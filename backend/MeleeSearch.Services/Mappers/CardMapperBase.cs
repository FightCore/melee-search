using System.Text.Json;
using MeleeSearch.Models.Entities;
using MeleeSearch.Repositories.DTOs;

namespace MeleeSearch.Services.Mappers;

public abstract class CardMapperBase : ICardMapper
{
    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public abstract string SupportedType { get; }

    public SearchCardDto MapToCard(DataEntry entry)
    {
        var card = new SearchCardDto
        {
            Id = entry.Id,
            Type = SupportedType,
            Title = entry.Title,
            Tags = [],
            CreatedAt = entry.CreatedAt,
            UpdatedAt = entry.UpdatedAt,
            Image = entry.Image,
        };

        try
        {
            var dictionaryData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(entry.Data, JsonOptions);
            var jsonDictionary = new Dictionary<string, JsonElement>();
            foreach (var (key, value) in dictionaryData)
            {
                var newKey = char.ToLower(key[0]) +  key[1..];
                jsonDictionary.Add(newKey, value);
            }

            card.JsonData = jsonDictionary;
            var jsonData = JsonSerializer.Deserialize<JsonElement>(entry.Data, JsonOptions);
            PopulateCardFromJson(card, jsonData);
        }
        catch (JsonException)
        {
        }

        return card;
    }

    protected abstract void PopulateCardFromJson(SearchCardDto card, JsonElement jsonData);

    protected static string? GetStringProperty(JsonElement json, string propertyName)
    {
        if (!json.TryGetProperty(propertyName, out var element) ||
            element.ValueKind != JsonValueKind.String)
        {
            return null;
        }
        var value = element.GetString();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
    
    protected static int? GetIntProperty(JsonElement json, string propertyName)
    {
        json.TryGetProperty(propertyName, out var element);

        return element.ValueKind switch
        {
            JsonValueKind.Null => null,
            JsonValueKind.Number => element.GetInt32(),
            _ => null,
        };
    }

    protected static List<CardLink>? GetLinksArray(JsonElement json, string propertyName)
    {
        if (!json.TryGetProperty(propertyName, out var element) ||
            element.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        var links = new List<CardLink>();
        foreach (var item in element.EnumerateArray())
        {
            var description = GetStringProperty(item, "description");
            var url = GetStringProperty(item, "url");

            if (!string.IsNullOrWhiteSpace(description) && !string.IsNullOrWhiteSpace(url))
            {
                links.Add(new CardLink { Description = description, Url = url });
            }
        }

        return links.Count > 0 ? links : null;
    }
}
