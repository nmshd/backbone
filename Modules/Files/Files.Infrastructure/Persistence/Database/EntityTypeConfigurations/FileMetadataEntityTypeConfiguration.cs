using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Files.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Files.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class FileMetadataEntityTypeConfiguration : IEntityTypeConfiguration<FileMetadata>
{
    public void Configure(EntityTypeBuilder<FileMetadata> builder)
    {
        builder.HasIndex(m => m.CreatedBy);

        builder.Property(m => m.CipherHash).IsRequired();
    }
}
