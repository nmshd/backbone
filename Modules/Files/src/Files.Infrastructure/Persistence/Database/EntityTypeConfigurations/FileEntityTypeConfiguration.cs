using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class FileEntityTypeConfiguration : IEntityTypeConfiguration<File>
{
    public void Configure(EntityTypeBuilder<File> builder)
    {
        builder.ToTable("FileMetadata");
        
        builder.HasIndex(m => m.CreatedBy);

        builder.Property(m => m.CipherHash).IsRequired();

        builder.Ignore(m => m.Content);
    }
}
