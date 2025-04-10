using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Devices.Domain.Aggregates.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.EntityConfigurations;

public class RelationshipTypeConfiguration : EntityEntityTypeConfiguration<Relationship>
{
    public override void Configure(EntityTypeBuilder<Relationship> builder)
    {
        base.Configure(builder);

        builder.ToTable("Relationships", "Relationships", x => x.ExcludeFromMigrations());

        builder.HasKey(x => x.Id);

        builder.Property(x => x.From);
        builder.Property(x => x.To);
    }
}
