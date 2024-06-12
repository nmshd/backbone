using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Relationships.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class RelationshipEntityTypeConfiguration : EntityEntityTypeConfiguration<Relationship>
{
    public override void Configure(EntityTypeBuilder<Relationship> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => x.From);
        builder.HasIndex(x => x.To);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RelationshipTemplateId);
        builder.Property(x => x.CreatedAt);

        builder.Property(x => x.CreationContent);
        builder.Property(x => x.CreationResponseContent);
    }
}
