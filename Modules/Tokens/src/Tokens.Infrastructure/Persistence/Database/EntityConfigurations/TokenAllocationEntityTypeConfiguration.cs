using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Tokens.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Tokens.Infrastructure.Persistence.Database.EntityConfigurations;

public class TokenAllocationEntityTypeConfiguration : EntityEntityTypeConfiguration<TokenAllocation>
{
    public override void Configure(EntityTypeBuilder<TokenAllocation> builder)
    {
        base.Configure(builder);

        builder.ToTable("TokenAllocations");
        builder.HasKey(x => new { x.TokenId, x.AllocatedBy });

        builder.Property(x => x.AllocatedAt);
        builder.Property(x => x.AllocatedByDevice);
    }
}
