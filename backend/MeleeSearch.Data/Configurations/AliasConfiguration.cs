using MeleeSearch.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeleeSearch.Data.Configurations;

public class AliasConfiguration : IEntityTypeConfiguration<Alias>
{
    public void Configure(EntityTypeBuilder<Alias> builder)
    {
        builder.ToTable("aliases");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.Term)
            .HasColumnName("term")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Replacement)
            .HasColumnName("replacement")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.HasIndex(x => x.Term)
            .IsUnique()
            .HasDatabaseName("idx_aliases_term");
    }
}
