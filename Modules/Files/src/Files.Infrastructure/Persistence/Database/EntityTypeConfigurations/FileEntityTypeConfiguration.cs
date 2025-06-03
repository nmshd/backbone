using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class FileEntityTypeConfiguration : EntityEntityTypeConfiguration<File>
{
    public override void Configure(EntityTypeBuilder<File> builder)
    {
        base.Configure(builder);

        builder.ToTable("FileMetadata");

        builder.Ignore(m => m.Content);

        builder.Property(m => m.CipherHash).IsRequired();

        builder.Property(m => m.OwnershipIsLocked).HasDefaultValue(true);
    }
}
