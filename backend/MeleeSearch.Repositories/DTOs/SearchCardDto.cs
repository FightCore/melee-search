using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeleeSearch.Repositories.DTOs;

public class SearchCardDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Image { get; set; }
    public string? Description { get; set; }
    public List<CardLink>? Links { get; set; }
    public List<string> Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? JsonData { get; set; }
}

public class CardLink
{
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
