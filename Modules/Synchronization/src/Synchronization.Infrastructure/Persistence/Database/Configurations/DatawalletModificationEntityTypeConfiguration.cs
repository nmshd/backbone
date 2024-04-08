using Backbone.Modules.Synchronization.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.Configurations;

public class DatawalletModificationEntityTypeConfiguration : IEntityTypeConfiguration<DatawalletModification>
{
    public void Configure(EntityTypeBuilder<DatawalletModification> builder)
    {
        builder.HasIndex(p => new { p.CreatedBy, p.Index }).IsUnique();

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedBy);
        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.CreatedByDevice);
        builder.Property(x => x.DatawalletVersion);

        builder.Property(x => x.Collection).HasMaxLength(50);
        builder.Property(x => x.ObjectIdentifier).HasMaxLength(100);
        builder.Property(x => x.PayloadCategory).HasMaxLength(50);
        builder.Property(x => x.Type);
        builder.Property(x => x.BlobReference).HasMaxLength(32).IsUnicode(false).IsFixedLength(true);
        builder.Property(x => x.EncryptedPayload);
    }
}
