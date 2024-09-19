using Backbone.Modules.Quotas.Domain.Aggregates.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;

public class RelationshipTemplateEntityTypeConfiguration : IEntityTypeConfiguration<RelationshipTemplate>
{
    public void Configure(EntityTypeBuilder<RelationshipTemplate> builder)
    {
        builder.ToTable(nameof(RelationshipTemplate) + "s", "Relationships", x => x.ExcludeFromMigrations());
        builder.HasNoKey();
    }
}
