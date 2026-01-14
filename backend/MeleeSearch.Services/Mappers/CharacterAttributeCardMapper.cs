using System.Text.Json;
using MeleeSearch.Repositories.DTOs;

namespace MeleeSearch.Services.Mappers;

public class CharacterAttributeCardMapper : CardMapperBase
{
    public override string SupportedType => "character_attribute";

    protected override void PopulateCardFromJson(SearchCardDto card, JsonElement jsonData)
    {
        card.Image = GetStringProperty(jsonData, "character_image")
                     ?? GetStringProperty(jsonData, "portrait")
                     ?? GetStringProperty(jsonData, "image");

        card.Description = GetStringProperty(jsonData, "description")
                          ?? GetStringProperty(jsonData, "attribute_description")
                          ?? BuildDescriptionFromAttributes(jsonData);

        card.Links = GetLinksArray(jsonData, "links")
                    ?? GetLinksArray(jsonData, "wiki_links");
    }

    private static string? BuildDescriptionFromAttributes(JsonElement jsonData)
    {
        var attributes = new List<string>();

        if (jsonData.TryGetProperty("weight", out var weight))
        {
            attributes.Add($"Weight: {weight}");
        }

        if (jsonData.TryGetProperty("speed", out var speed))
        {
            attributes.Add($"Speed: {speed}");
        }

        if (jsonData.TryGetProperty("jump_height", out var jumpHeight))
        {
            attributes.Add($"Jump Height: {jumpHeight}");
        }

        return attributes.Count > 0 ? string.Join(", ", attributes) : null;
    }
}
