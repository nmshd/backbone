using Backbone.Modules.Relationships.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Relationships.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class RelationshipChangeResponseEntityTypeConfiguration : IEntityTypeConfiguration<RelationshipChangeResponse>
{
    public void Configure(EntityTypeBuilder<RelationshipChangeResponse> builder)
    {
        builder.ToTable("RelationshipChanges");

        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.CreatedBy);
        builder.HasIndex(x => x.CreatedByDevice);

        var indexBuilder = builder.HasIndex(x => new { x.CreatedAt, x.CreatedBy, x.CreatedByDevice });
        NpgsqlIndexBuilderExtensions.IncludeProperties(indexBuilder, x => x.Content!);
        SqlServerIndexBuilderExtensions.IncludeProperties(indexBuilder, x => x.Content);

        builder.Property(x => x.CreatedBy)
            .HasColumnName("Res_CreatedBy");

        builder.Property(x => x.CreatedByDevice)
            .HasColumnName("Res_CreatedByDevice");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("Res_CreatedAt");

        builder.Property(x => x.Content)
            .HasColumnName("Res_Content");
    }
}
