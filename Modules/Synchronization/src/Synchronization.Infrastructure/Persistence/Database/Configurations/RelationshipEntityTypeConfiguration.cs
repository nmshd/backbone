using Backbone.Modules.Synchronization.Domain.Entities.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.Configurations;

public class RelationshipEntityTypeConfiguration : IEntityTypeConfiguration<Relationship>
{
    public void Configure(EntityTypeBuilder<Relationship> builder)
    {
        builder.ToTable("Relationships", "Relationships", x => x.ExcludeFromMigrations());

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status);
        builder.Property(x => x.From);
        builder.Property(x => x.To);
    }
}
