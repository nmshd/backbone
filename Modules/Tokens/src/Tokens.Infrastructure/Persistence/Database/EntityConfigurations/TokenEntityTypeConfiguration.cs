using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Tokens.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Tokens.Infrastructure.Persistence.Database.EntityConfigurations;

public class TokenEntityTypeConfiguration : EntityEntityTypeConfiguration<Token>, IEntityTypeConfiguration<TokenDetails>
{
    public override void Configure(EntityTypeBuilder<Token> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Password).HasMaxLength(Token.MAX_PASSWORD_LENGTH);

        builder.HasMany(x => x.Allocations).WithOne().OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Details).WithOne().HasForeignKey<TokenDetails>(x => x.Id).IsRequired();

        builder.HasVersion(x => x.Version);
    }

    public void Configure(EntityTypeBuilder<TokenDetails> builder)
    {
        builder.ToTable("Tokens");
    }
}
