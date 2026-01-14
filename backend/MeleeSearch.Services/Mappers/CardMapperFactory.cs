using MeleeSearch.Models.Entities;

namespace MeleeSearch.Services.Mappers;

public interface ICardMapperFactory
{
    ICardMapper GetMapper(DataEntry entry);
}

public class CardMapperFactory : ICardMapperFactory
{
    private readonly Dictionary<string, ICardMapper> _mappers;
    private readonly ICardMapper _defaultMapper;

    public CardMapperFactory(IEnumerable<ICardMapper> mappers)
    {
        _mappers = mappers.ToDictionary(m => m.SupportedType, m => m);
        _defaultMapper = _mappers.GetValueOrDefault("unknown") ?? new DefaultCardMapper();
    }

    public ICardMapper GetMapper(DataEntry entry)
    {
        var type = entry switch
        {
            FrameData => "frame_data",
            CharacterAttribute => "character_attribute",
            _ => "unknown"
        };

        return _mappers.GetValueOrDefault(type) ?? _defaultMapper;
    }
}
