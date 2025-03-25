using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Devices.Domain.Aggregates.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.EntityConfigurations;

public class RelationshipTemplateEntityTypeConfiguration : EntityEntityTypeConfiguration<RelationshipTemplate>
{
    public override void Configure(EntityTypeBuilder<RelationshipTemplate> builder)
    {
        base.Configure(builder);

        builder.ToTable("RelationshipTemplates", "Relationships", x => x.ExcludeFromMigrations());

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedBy);

        builder.HasMany(x => x.Allocations).WithOne(x => x.RelationshipTemplate);
    }
}
