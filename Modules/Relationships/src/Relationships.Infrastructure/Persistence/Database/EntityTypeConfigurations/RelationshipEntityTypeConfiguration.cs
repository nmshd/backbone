using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Relationships.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class RelationshipEntityTypeConfiguration : IEntityTypeConfiguration<Relationship>
{
    public void Configure(EntityTypeBuilder<Relationship> builder)
    {
        builder.HasIndex(x => x.From);
        builder.HasIndex(x => x.To);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.Status);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RelationshipTemplateId);
    }
}
