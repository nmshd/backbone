using Backbone.Modules.Files.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Files.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class FileMetadataEntityTypeConfiguration : IEntityTypeConfiguration<FileMetadata>
{
    public void Configure(EntityTypeBuilder<FileMetadata> builder)
    {
        builder.HasIndex(m => m.CreatedBy);

        builder.Property(m => m.CipherHash).IsRequired();

        builder.Ignore(m => m.Content);
    }
}
