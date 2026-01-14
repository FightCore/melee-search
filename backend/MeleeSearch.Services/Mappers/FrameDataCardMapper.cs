using System.Text.Json;
using MeleeSearch.Repositories.DTOs;

namespace MeleeSearch.Services.Mappers;

public class FrameDataCardMapper : CardMapperBase
{
    public override string SupportedType => "frame_data";

    protected override void PopulateCardFromJson(SearchCardDto card, JsonElement jsonData)
    {
        card.Image = GetStringProperty(jsonData, "GifUrl");
        
        var moveId = GetIntProperty(jsonData, "MoveId");
        var normalizedMoveName = GetStringProperty(jsonData, "NormalizedMoveName");
        var characterId = GetIntProperty(jsonData, "CharacterId");
        var normalizedCharacterName = GetStringProperty(jsonData, "NormalizedCharacterName");
        
        
        var start = GetIntProperty(jsonData, "Start");
        var end = GetIntProperty(jsonData, "End");
        var totalFrames = GetIntProperty(jsonData, "TotalFrames");
        card.Description = $"Start: {start}, End: {end}, TotalFrames: {totalFrames}";
        
        card.Links =
        [
            new()
            {
                Description = "Visit Fightcore",
                Url = $"https://www.fightcore.gg/characters/{characterId}/{normalizedCharacterName}/moves/{moveId}/{normalizedMoveName}/"
            }
        ];
    }
}
