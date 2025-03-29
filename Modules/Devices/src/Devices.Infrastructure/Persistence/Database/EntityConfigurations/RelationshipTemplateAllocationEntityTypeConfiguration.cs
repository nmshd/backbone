using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Devices.Domain.Aggregates.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.EntityConfigurations;

public class RelationshipTemplateAllocationEntityTypeConfiguration : EntityEntityTypeConfiguration<RelationshipTemplateAllocation>
{
    public override void Configure(EntityTypeBuilder<RelationshipTemplateAllocation> builder)
    {
        base.Configure(builder);

        builder.ToTable("RelationshipTemplateAllocations", "Relationships", x => x.ExcludeFromMigrations());

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AllocatedBy);

        builder.HasOne(x => x.RelationshipTemplate).WithMany(x => x.Allocations);
    }
}
