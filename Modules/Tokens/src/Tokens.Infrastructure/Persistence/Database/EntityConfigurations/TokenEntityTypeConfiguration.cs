using Backbone.Tokens.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Tokens.Infrastructure.Persistence.Database.EntityConfigurations;

public class TokenEntityTypeConfiguration : IEntityTypeConfiguration<Token>
{
    public void Configure(EntityTypeBuilder<Token> builder)
    {
        builder.HasIndex(r => r.CreatedBy);

        builder.Ignore(a => a.Content);
    }
}
