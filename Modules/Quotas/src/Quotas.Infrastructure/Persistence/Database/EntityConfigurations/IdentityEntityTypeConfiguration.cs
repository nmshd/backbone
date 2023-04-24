using Backbone.Modules.Quotas.Domain.Aggregates.Entities;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;

public class IdentityEntityTypeConfiguration : IEntityTypeConfiguration<Identity>
{
    public void Configure(EntityTypeBuilder<Identity> builder)
    {
        builder.HasKey(x => x.Address);

        builder.Property(x => x.Address).IsUnicode(false).IsFixedLength().HasMaxLength(IdentityAddress.MAX_LENGTH);
        builder.Property(x => x.TierId).IsUnicode(false).IsFixedLength().HasMaxLength(20);
        
    }
}
