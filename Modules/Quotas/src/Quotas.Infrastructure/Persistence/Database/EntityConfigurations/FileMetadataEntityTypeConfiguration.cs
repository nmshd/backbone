using Backbone.Modules.Quotas.Domain.Aggregates.FileMetadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;

public class FileMetadataEntityTypeConfiguration : IEntityTypeConfiguration<FileMetadata>
{
    public void Configure(EntityTypeBuilder<FileMetadata> builder)
    {
        builder.ToTable(nameof(FileMetadata), "Files", x => x.ExcludeFromMigrations());
        builder.HasKey(x => x.Id);
    }
}
