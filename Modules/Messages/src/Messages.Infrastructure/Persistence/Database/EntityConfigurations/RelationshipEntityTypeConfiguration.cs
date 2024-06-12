using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Messages.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Messages.Infrastructure.Persistence.Database.EntityConfigurations;

public class RelationshipEntityTypeConfiguration : EntityEntityTypeConfiguration<Relationship>
{
    public override void Configure(EntityTypeBuilder<Relationship> builder)
    {
        base.Configure(builder);

        builder.HasKey(r => r.Id);

        builder.ToTable(nameof(Relationship) + "s", "Relationships", x => x.ExcludeFromMigrations());

        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.From);
        builder.Property(x => x.To);
        builder.Property(x => x.Status);
    }
}
