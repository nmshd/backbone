using Backbone.Modules.Relationships.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Relationships.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class RelationshipChangeEntityTypeConfiguration : IEntityTypeConfiguration<RelationshipChange>
{
    public void Configure(EntityTypeBuilder<RelationshipChange> builder)
    {
        builder.ToTable("RelationshipChanges");

        builder.Ignore(x => x.IsCompleted);
        builder.Ignore(x => x.Request);
        builder.Ignore(x => x.Response);

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
