namespace MeleeSearch.Models.Entities;

public class Tag : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ICollection<DataEntry> DataEntries { get; set; } = new List<DataEntry>();
}
