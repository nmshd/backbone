using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Messages.Domain.Entities;
using Messages.Domain.Ids;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messages.Infrastructure.Persistence.Database.EntityConfigurations;

public class RelationshipEntityTypeConfiguration : IEntityTypeConfiguration<Relationship>
{
    public void Configure(EntityTypeBuilder<Relationship> builder)
    {
        builder.HasKey(r => r.Id);

        builder.ToTable(nameof(Relationship) + "s", "Relationships");

        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.From);
        builder.Property(x => x.To);
    }
}
