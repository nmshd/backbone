using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.EntityConfigurations;

public class IdentityEntityTypeConfiguration : IEntityTypeConfiguration<Identity>
{
    public void Configure(EntityTypeBuilder<Identity> builder)
    {
        builder.HasKey(x => x.Address);
        builder.Property(x => x.ClientId).HasMaxLength(200);

        // When migrating from an old version, an Identity may not be associated with a tier yet,
        // which is why the foreign key column is marked as not required.
        builder.HasOne<Tier>().WithMany(x => x.Identities).HasForeignKey(x => x.TierId).IsRequired(false);
    }
}
