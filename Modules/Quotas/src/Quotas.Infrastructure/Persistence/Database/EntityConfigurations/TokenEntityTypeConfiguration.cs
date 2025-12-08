using Backbone.Modules.Quotas.Domain.Aggregates.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;

public class TokenEntityTypeConfiguration : IEntityTypeConfiguration<Token>
{
    public void Configure(EntityTypeBuilder<Token> builder)
    {
        builder.ToTable(nameof(Token) + "s", "Tokens", x => x.ExcludeFromMigrations());
        builder.HasNoKey();
    }
}
