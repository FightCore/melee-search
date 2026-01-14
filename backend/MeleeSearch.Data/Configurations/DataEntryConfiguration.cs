using MeleeSearch.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeleeSearch.Data.Configurations;

public class DataEntryConfiguration : IEntityTypeConfiguration<DataEntry>
{
    public void Configure(EntityTypeBuilder<DataEntry> builder)
    {
        builder.ToTable("data_entries");

        builder.HasDiscriminator<string>("type")
            .HasValue<FrameData>("frame_data")
            .HasValue<CharacterAttribute>("character_attribute");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Data)
            .HasColumnName("data")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.HasMany(x => x.Tags)
            .WithMany(x => x.DataEntries)
            .UsingEntity<Dictionary<string, object>>(
                "data_entry_tags",
                j => j.HasOne<Tag>().WithMany().HasForeignKey("tag_id").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<DataEntry>().WithMany().HasForeignKey("data_entry_id").OnDelete(DeleteBehavior.Cascade),
                j => j.HasKey("data_entry_id", "tag_id"));

        builder.HasMany(x => x.Characters)
            .WithMany(x => x.DataEntries)
            .UsingEntity<Dictionary<string, object>>(
                "data_entry_characters",
                j => j.HasOne<Character>().WithMany().HasForeignKey("character_id").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<DataEntry>().WithMany().HasForeignKey("data_entry_id").OnDelete(DeleteBehavior.Cascade),
                j => j.HasKey("data_entry_id", "character_id"));

        builder.HasIndex("type").HasDatabaseName("idx_data_entries_type");
        builder.HasIndex(x => x.Title).HasDatabaseName("idx_data_entries_title");
        builder.HasIndex(x => x.CreatedAt).HasDatabaseName("idx_data_entries_created_at");
    }
}
