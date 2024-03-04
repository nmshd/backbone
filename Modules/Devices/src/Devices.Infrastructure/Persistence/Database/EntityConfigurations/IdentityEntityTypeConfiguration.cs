using Backbone.Modules.Devices.Domain.Entities.Identities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.EntityConfigurations;

public class IdentityEntityTypeConfiguration : IEntityTypeConfiguration<Identity>
{
    public void Configure(EntityTypeBuilder<Identity> builder)
    {
        builder.HasKey(x => x.Address);

        builder.Property(x => x.ClientId).HasMaxLength(200);
        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.PublicKey);

        builder.HasMany(x => x.DeletionProcesses).WithOne().OnDelete(DeleteBehavior.Cascade);
    }
}
