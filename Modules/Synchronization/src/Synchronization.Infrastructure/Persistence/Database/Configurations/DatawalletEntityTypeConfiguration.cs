using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Synchronization.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.Configurations;

public class DatawalletEntityTypeConfiguration : EntityEntityTypeConfiguration<Datawallet>
{
    public override void Configure(EntityTypeBuilder<Datawallet> builder)
    {
        base.Configure(builder);

        builder.HasIndex(p => p.Owner).IsUnique();

        builder.HasKey(x => x.Id);

        builder.HasMany(dw => dw.Modifications).WithOne(m => m.Datawallet).OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(x => x.LatestModification);
    }
}
