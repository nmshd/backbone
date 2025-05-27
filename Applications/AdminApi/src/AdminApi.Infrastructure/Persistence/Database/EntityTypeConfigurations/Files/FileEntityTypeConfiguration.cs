using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = Backbone.AdminApi.Infrastructure.Persistence.Models.Files.File;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database.EntityTypeConfigurations.Files;

public class FileEntityTypeConfiguration : IEntityTypeConfiguration<File>
{
    public void Configure(EntityTypeBuilder<File> builder)
    {
        builder.ToTable("FileMetadata", "Files", t => t.ExcludeFromMigrations());

        builder.HasKey(x => x.Id);
    }
}
