using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Domain.Aggregates.Devices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Persistence.Database.EntityConfigurations.Devices;
public class IdentityEntityTypeConfiguration : IEntityTypeConfiguration<Identity>
{
    public void Configure(EntityTypeBuilder<Identity> builder)
    {
        builder.ToTable("Identities", "Devices", x=>x.ExcludeFromMigrations());
        builder.HasKey(x => x.Address);
    }
}
