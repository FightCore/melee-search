using MeleeSearch.Models.Entities;
using MeleeSearch.Repositories.DTOs;

namespace MeleeSearch.Services.Mappers;

public interface ICardMapper
{
    string SupportedType { get; }
    SearchCardDto MapToCard(DataEntry entry);
}
