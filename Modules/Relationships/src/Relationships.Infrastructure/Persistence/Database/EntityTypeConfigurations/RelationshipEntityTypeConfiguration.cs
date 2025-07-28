using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Relationships.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class RelationshipEntityTypeConfiguration : EntityEntityTypeConfiguration<Relationship>, IEntityTypeConfiguration<RelationshipDetails>
{
    public override void Configure(EntityTypeBuilder<Relationship> builder)
    {
        base.Configure(builder);

        base.Configure(builder);

        builder.HasIndex(x => x.From).HasMethod("hash");
        builder.HasIndex(x => x.To).HasMethod("hash");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RelationshipTemplateId);
        builder.Property(x => x.CreatedAt);

        builder.HasOne(x => x.Details).WithOne().HasForeignKey<RelationshipDetails>(x => x.Id).IsRequired();

        builder.HasMany(r => r.AuditLog).WithOne().OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<RelationshipDetails> builder)
    {
        builder.ToTable("Relationships");
    }
}
