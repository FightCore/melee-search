using System.Text.Json;
using MeleeSearch.Repositories.DTOs;

namespace MeleeSearch.Services.Mappers;

public class DefaultCardMapper : CardMapperBase
{
    public override string SupportedType => "unknown";

    protected override void PopulateCardFromJson(SearchCardDto card, JsonElement jsonData)
    {
        card.Image = GetStringProperty(jsonData, "image");
        card.Description = GetStringProperty(jsonData, "description");
        card.Links = GetLinksArray(jsonData, "links");
    }
}
