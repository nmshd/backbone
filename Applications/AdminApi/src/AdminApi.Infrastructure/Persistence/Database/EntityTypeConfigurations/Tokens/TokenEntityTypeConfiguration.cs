using Backbone.AdminApi.Infrastructure.Persistence.Models.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database.EntityTypeConfigurations.Tokens;

public class TokenEntityTypeConfiguration : IEntityTypeConfiguration<Token>
{
    public void Configure(EntityTypeBuilder<Token> builder)
    {
        builder.ToTable("Tokens", "Tokens", t => t.ExcludeFromMigrations());

        builder.HasKey(x => x.Id);
    }
}
