using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Relationships.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class RelationshipTemplateEntityTypeConfiguration : EntityEntityTypeConfiguration<RelationshipTemplate>, IEntityTypeConfiguration<RelationshipTemplateDetails>
{
    public override void Configure(EntityTypeBuilder<RelationshipTemplate> builder)
    {
        base.Configure(builder);

        builder
            .HasMany(x => x.Relationships)
            .WithOne(x => x.RelationshipTemplate)
            .HasForeignKey(x => x.RelationshipTemplateId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasMany(x => x.Allocations)
            .WithOne(x => x.RelationshipTemplate)
            .HasForeignKey(x => x.RelationshipTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(x => x.Password)
            .HasMaxLength(RelationshipTemplate.MAX_PASSWORD_LENGTH);

        builder.HasOne(x => x.Details).WithOne().HasForeignKey<RelationshipTemplateDetails>(x => x.Id).IsRequired();

        var indexBuilder = builder.HasIndex(x => x.CreatedAt);
        SqlServerIndexBuilderExtensions.IncludeProperties(indexBuilder, x => x.CreatedBy);
        NpgsqlIndexBuilderExtensions.IncludeProperties(indexBuilder, x => x.CreatedBy);
    }

    public void Configure(EntityTypeBuilder<RelationshipTemplateDetails> builder)
    {
        builder.ToTable("RelationshipTemplates");
    }
}
