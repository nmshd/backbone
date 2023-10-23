using Backbone.Quotas.Domain.Aggregates.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;
public class RelationshipEntityTypeConfiguration : IEntityTypeConfiguration<Relationship>
{
    public void Configure(EntityTypeBuilder<Relationship> builder)
    {
        builder.ToTable(nameof(Relationship) + "s", "Relationships", x => x.ExcludeFromMigrations());
        builder.HasKey(x => x.Id);
    }
}
