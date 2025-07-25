using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Synchronization.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.Configurations;

public class DatawalletModificationEntityTypeConfiguration : EntityEntityTypeConfiguration<DatawalletModification>, IEntityTypeConfiguration<DatawalletModificationDetails>
{
    public override void Configure(EntityTypeBuilder<DatawalletModification> builder)
    {
        base.Configure(builder);

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
        builder.HasOne(x => x.Details).WithOne().HasForeignKey<DatawalletModificationDetails>(x => x.Id).IsRequired();
    }

    public void Configure(EntityTypeBuilder<DatawalletModificationDetails> builder)
    {
        builder.ToTable("DatawalletModifications");
    }
}
