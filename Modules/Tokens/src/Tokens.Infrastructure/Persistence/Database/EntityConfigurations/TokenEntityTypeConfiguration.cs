using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Tokens.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Tokens.Infrastructure.Persistence.Database.EntityConfigurations;

public class TokenEntityTypeConfiguration : EntityEntityTypeConfiguration<Token>
{
    public override void Configure(EntityTypeBuilder<Token> builder)
    {
        base.Configure(builder);

        builder.Property(r => r.Content).IsRequired(false);
    }
}
