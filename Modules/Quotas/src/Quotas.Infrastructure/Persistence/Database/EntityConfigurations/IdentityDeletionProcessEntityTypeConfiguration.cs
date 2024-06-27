using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;

public class IdentityDeletionProcessEntityTypeConfiguration : IEntityTypeConfiguration<IdentityDeletionProcess>
{
    public void Configure(EntityTypeBuilder<IdentityDeletionProcess> builder)
    {
        builder.ToTable($"{nameof(IdentityDeletionProcess)}es", "Devices", x => x.ExcludeFromMigrations());
        builder.HasKey(x => x.Id);
    }
}
