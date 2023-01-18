using Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database.ValueConverters;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Relationships.Domain.Entities;

namespace Relationships.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class RelationshipChangeRequestEntityTypeConfiguration : IEntityTypeConfiguration<RelationshipChangeRequest>
{
    public void Configure(EntityTypeBuilder<RelationshipChangeRequest> builder)
    {
        builder.ToTable("RelationshipChanges");

        builder.Ignore(x => x.Content);

        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.CreatedBy);
        builder.HasIndex(x => x.CreatedByDevice);

        builder.Property(x => x.CreatedBy)
            .HasColumnName("Req_CreatedBy");

        builder.Property(x => x.CreatedByDevice)
            .HasColumnName("Req_CreatedByDevice");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("Req_CreatedAt");
    }
}
