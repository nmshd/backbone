using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Relationships.Domain.Entities;
using Relationships.Domain.Ids;
using Relationships.Infrastructure.Persistence.Database.ValueConverters;

namespace Relationships.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class RelationshipChangeEntityTypeConfiguration : IEntityTypeConfiguration<RelationshipChange>
{
    public void Configure(EntityTypeBuilder<RelationshipChange> builder)
    {
        builder.ToTable("RelationshipChanges");

        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.Type);
        builder.HasIndex(x => x.CreatedAt);

        builder.Ignore(x => x.IsCompleted);
        builder.Ignore(x => x.Request);
        builder.Ignore(x => x.Response);

        builder.Ignore("Req_Content");
        builder.Ignore("Res_Content");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RelationshipId);

        builder.HasOne(x => x.Relationship).WithMany(x => x.Changes);
        
        builder
            .HasOne(x => x.Request).WithOne()
            .HasForeignKey<RelationshipChangeRequest>(b => b.Id);

        builder
            .HasOne(x => x.Response).WithOne()
            .HasForeignKey<RelationshipChangeResponse>(b => b.Id);
    }
}

public class RelationshipCreationChangeEntityTypeConfiguration : IEntityTypeConfiguration<RelationshipCreationChange>
{
    public void Configure(EntityTypeBuilder<RelationshipCreationChange> builder)
    {
        builder.ToTable("RelationshipChanges");
    }
}

public class RelationshipTerminationChangeEntityTypeConfiguration : IEntityTypeConfiguration<RelationshipTerminationChange>
{
    public void Configure(EntityTypeBuilder<RelationshipTerminationChange> builder)
    {
        builder.ToTable("RelationshipChanges");
    }
}
