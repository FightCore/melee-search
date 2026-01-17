using System.Text.Json;
using MeleeSearch.Repositories.DTOs;

namespace MeleeSearch.Services.Mappers;

public class FrameDataCardMapper : CardMapperBase
{
    public override string SupportedType => "frame_data";

    protected override void PopulateCardFromJson(SearchCardDto card, JsonElement jsonData)
    {
        var moveId = GetIntProperty(jsonData, "MoveId");
        var normalizedMoveName = GetStringProperty(jsonData, "NormalizedMoveName");
        var characterId = GetIntProperty(jsonData, "CharacterId");
        var normalizedCharacterName = GetStringProperty(jsonData, "NormalizedCharacterName");
        
        card.Links =
        [
            new()
            {
                Description = "Fightcore",
                Url = $"https://www.fightcore.gg/characters/{characterId}/{normalizedCharacterName}/moves/{moveId}/{normalizedMoveName}/"
            }
        ];
    }
}
