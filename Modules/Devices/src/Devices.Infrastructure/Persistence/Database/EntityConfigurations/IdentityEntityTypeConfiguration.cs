using Devices.Domain.Entities;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Devices.Infrastructure.Persistence.Database.EntityConfigurations;

public class IdentityEntityTypeConfiguration : IEntityTypeConfiguration<Identity>
{
    public void Configure(EntityTypeBuilder<Identity> builder)
    {
        builder.HasKey(x => x.Address);
        
        builder.Property(x => x.ClientId).HasMaxLength(200);
    }
}
