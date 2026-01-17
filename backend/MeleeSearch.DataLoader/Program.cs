using System.Text.Json;
using System.Text.RegularExpressions;
using MeleeSearch.Data.Context;
using MeleeSearch.DataLoader;
using MeleeSearch.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

const string jsonFilePath = @"C:\tmp\exports\MeleeSearch\data-entries.json";
const string dataType = "framedata";
const string connectionString = "Host=localhost;Database=meleesearch;Username=postgres;Password=localcontainer";

if (dataType != "aliases" && !File.Exists(jsonFilePath))
{
    Console.WriteLine($"Error: File not found: {jsonFilePath}");
    return;
}

var optionsBuilder = new DbContextOptionsBuilder<MeleeSearchDbContext>();
optionsBuilder.UseNpgsql(connectionString);

using var context = new MeleeSearchDbContext(optionsBuilder.Options);

try
{
    switch (dataType)
    {
        case "aliases":
            Console.WriteLine("Seeding aliases...");
            await AliasSeeder.SeedAliasesAsync(context);
            break;

        case "framedata":
            Console.WriteLine("Reading JSON file...");
            var jsonContent = await File.ReadAllTextAsync(jsonFilePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var frameDataItems = JsonSerializer.Deserialize<List<JsonDataEntry>>(jsonContent, options);
            if (frameDataItems != null)
            {
                Console.WriteLine($"Found {frameDataItems.Count} frame data items");
                await LoadFrameData(context, frameDataItems);
            }
            break;

        case "characterattribute":
            {
                Console.WriteLine("Reading JSON file...");
                var jsonContent2 = await File.ReadAllTextAsync(jsonFilePath);
                var options2 = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var attributeItems = JsonSerializer.Deserialize<List<JsonDataEntry>>(jsonContent2, options2);
                if (attributeItems != null)
                {
                    Console.WriteLine($"Found {attributeItems.Count} character attribute items");
                    await LoadCharacterAttributes(context, attributeItems);
                }
            }
            break;

        default:
            Console.WriteLine($"Unknown data type: {dataType}");
            return;
    }

    Console.WriteLine("Data loaded successfully!");
}
catch (Exception ex)
{
    Console.WriteLine($"Error loading data: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
}

static async Task LoadFrameData(MeleeSearchDbContext context, List<JsonDataEntry> items)
{
    var now = DateTime.UtcNow;

    foreach (var item in items)
    {
        var tags = await GetOrCreateTags(context, item.Tags);
        var characters = await GetOrCreateCharacters(context, item.Tags, item.Title);

        var frameData = new FrameData
        {
            Title = item.Title,
            Data = JsonSerializer.Serialize(item.Data),
            Tags = tags,
            Characters = characters,
            CreatedAt = now,
            UpdatedAt = now,
            Image = item.Image
        };

        context.FrameData.Add(frameData);
    }

    Console.WriteLine("Saving to database...");
    await context.SaveChangesAsync();
    Console.WriteLine($"Saved {items.Count} frame data entries");
}

static async Task LoadCharacterAttributes(MeleeSearchDbContext context, List<JsonDataEntry> items)
{
    var now = DateTime.UtcNow;

    foreach (var item in items)
    {
        var tags = await GetOrCreateTags(context, item.Tags);
        var characters = await GetOrCreateCharacters(context, item.Tags, item.Title);

        var attribute = new CharacterAttribute
        {
            Title = item.Title,
            Data = JsonSerializer.Serialize(item.Data),
            Tags = tags,
            Characters = characters,
            CreatedAt = now,
            UpdatedAt = now
        };

        context.CharacterAttributes.Add(attribute);
    }

    Console.WriteLine("Saving to database...");
    await context.SaveChangesAsync();
    Console.WriteLine($"Saved {items.Count} character attribute entries");
}

static async Task<List<Tag>> GetOrCreateTags(MeleeSearchDbContext context, List<string>? tagNames)
{
    var tags = new List<Tag>();

    if (tagNames == null || tagNames.Count == 0)
        return tags;

    var now = DateTime.UtcNow;

    foreach (var tagName in tagNames.Distinct())
    {
        var existingTag = await context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);

        if (existingTag != null)
        {
            tags.Add(existingTag);
        }
        else
        {
            var newTag = new Tag
            {
                Name = tagName,
                CreatedAt = now,
                UpdatedAt = now
            };
            context.Tags.Add(newTag);
            tags.Add(newTag);
        }
    }

    await context.SaveChangesAsync();
    return tags;
}

static async Task<List<Character>> GetOrCreateCharacters(MeleeSearchDbContext context, List<string>? tagNames, string title)
{
    var characters = new List<Character>();

    // Complete list of Melee characters
    var meleeCharacters = new List<string>
    {
        "captainfalcon",
        "roy",
        "pichu",
        "ness",
        "mrgame&watch",
        "mewtwo",
        "link",
        "bowser",
        "kirby",
        "ganondorf",
        "donkeykong",
        "mario",
        "drmario",
        "luigi",
        "yoshi",
        "iceclimbers",
        "pikachu",
        "peach",
        "sheik",
        "samus",
        "jigglypuff",
        "marth",
        "fox",
        "falco",
        "younglink",
        "zelda",
        "fwireframe",
    };

    var foundCharacterNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    // Check tags for character names
    if (tagNames != null)
    {
        foreach (var tag in tagNames)
        {
            var normalizedTag = NormalizeText(tag);
            if (meleeCharacters.Contains(normalizedTag))
            {
                foundCharacterNames.Add(normalizedTag);
            }
        }
    }
    
    if (foundCharacterNames.Count == 0)
        return characters;

    var now = DateTime.UtcNow;

    // Fetch all characters once and filter in memory to avoid expression tree issues
    var allCharacters = await context.Characters.ToListAsync();

    foreach (var characterName in foundCharacterNames)
    {
        // Store normalized name in database for consistent matching
        var existingCharacter = allCharacters.FirstOrDefault(c =>
            NormalizeText(c.Name) == characterName);

        if (existingCharacter != null)
        {
            characters.Add(existingCharacter);
        }
        else
        {
            var newCharacter = new Character
            {
                Name = characterName,
                CreatedAt = now,
                UpdatedAt = now
            };
            context.Characters.Add(newCharacter);
            characters.Add(newCharacter);
        }
    }

    await context.SaveChangesAsync();
    return characters;
}

static string NormalizeText(string text)
{
    if (string.IsNullOrWhiteSpace(text))
    {
        return string.Empty;
    }

    // Convert to lowercase
    var normalized = text.ToLowerInvariant();

    // Remove special characters except spaces
    normalized = Regex.Replace(normalized, @"[^\w\s]", "");

    // Replace multiple spaces with single space
    normalized = Regex.Replace(normalized, @"\s+", " ");

    // Remove spaces entirely for consistent matching
    normalized = normalized.Replace(" ", "");

    return normalized.Trim();
}

public class JsonDataEntry
{
    public string Title { get; set; } = string.Empty;
    public object? Data { get; set; }
    public List<string>? Tags { get; set; }
    
    public string? Image { get; set; }
}
