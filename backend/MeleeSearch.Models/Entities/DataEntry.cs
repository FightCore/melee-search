namespace MeleeSearch.Models.Entities;

public abstract class DataEntry : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Data { get; set; } = "{}";
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public ICollection<Character> Characters { get; set; } = new List<Character>();
}
