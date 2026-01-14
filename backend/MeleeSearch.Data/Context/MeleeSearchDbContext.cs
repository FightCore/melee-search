using MeleeSearch.Data.Configurations;
using MeleeSearch.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeleeSearch.Data.Context;

public class MeleeSearchDbContext : DbContext
{
    public MeleeSearchDbContext(DbContextOptions<MeleeSearchDbContext> options)
        : base(options)
    {
    }

    public DbSet<DataEntry> DataEntries { get; set; }
    public DbSet<FrameData> FrameData { get; set; }
    public DbSet<CharacterAttribute> CharacterAttributes { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Character> Characters { get; set; }
    public DbSet<Alias> Aliases { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new DataEntryConfiguration());
        modelBuilder.ApplyConfiguration(new TagConfiguration());
        modelBuilder.ApplyConfiguration(new CharacterConfiguration());
        modelBuilder.ApplyConfiguration(new AliasConfiguration());
    }
}
