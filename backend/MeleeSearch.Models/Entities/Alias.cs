namespace MeleeSearch.Models.Entities;

public class Alias : BaseEntity
{
    public string Term { get; set; } = string.Empty;
    public string Replacement { get; set; } = string.Empty;
}
