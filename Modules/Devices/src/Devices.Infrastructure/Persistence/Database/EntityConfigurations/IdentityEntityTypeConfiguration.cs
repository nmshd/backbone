using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.EntityConfigurations;

public class IdentityEntityTypeConfiguration : EntityEntityTypeConfiguration<Identity>
{
    public override void Configure(EntityTypeBuilder<Identity> builder)
    {
        base.Configure(builder);
        builder.HasKey(x => x.Address);

        builder.Property(x => x.ClientId).HasMaxLength(200);
        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.PublicKey);
        builder.HasIndex(x => x.ClientId).HasMethod("hash");
        builder.HasIndex(x => x.TierId).HasMethod("hash");

        builder.HasMany(x => x.DeletionProcesses).WithOne().OnDelete(DeleteBehavior.Cascade);
    }
}
