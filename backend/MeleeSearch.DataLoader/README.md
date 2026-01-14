# MeleeSearch Data Loader

A simple console application to load JSON data into the MeleeSearch database.

## Usage

```bash
dotnet run --project MeleeSearch.DataLoader <json-file-path> <data-type>
```

### Parameters

- `json-file-path` - Path to the JSON file containing the data to load
- `data-type` - Type of data to load (options: `framedata`, `characterattribute`)

### Examples

Load frame data:
```bash
dotnet run --project MeleeSearch.DataLoader sample-data.json framedata
```

Load character attributes:
```bash
dotnet run --project MeleeSearch.DataLoader my-attributes.json characterattribute
```

## JSON Format

The JSON file should contain an array of objects with the following structure:

```json
[
  {
    "title": "Entry Title",
    "data": {
      // Any JSON object with your data
      "field1": "value1",
      "field2": 123
    },
    "tags": ["tag1", "tag2", "tag3"]
  }
]
```

### Example

```json
[
  {
    "title": "Fox Up Smash",
    "data": {
      "character": "Fox",
      "move": "Up Smash",
      "startup": 7,
      "totalFrames": 51,
      "damage": 18
    },
    "tags": ["fox", "smash-attack", "anti-air"]
  }
]
```

## Configuration

The database connection string can be configured in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=meleesearch;Username=postgres;Password=yourpassword"
  }
}
```

If no configuration file is found, it defaults to:
`Host=localhost;Database=meleesearch;Username=postgres;Password=localcontainer`

## Notes

- Tags are automatically created if they don't exist
- The `data` field can contain any JSON object structure
- The tool directly uses Entity Framework Core, bypassing repositories and services for simplicity
- Duplicate checking is not performed - running the tool multiple times will create duplicate entries
