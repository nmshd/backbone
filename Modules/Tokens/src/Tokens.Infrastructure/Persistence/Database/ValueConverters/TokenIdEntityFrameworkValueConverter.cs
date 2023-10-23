using Backbone.Tokens.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Tokens.Infrastructure.Persistence.Database.ValueConverters;

public class TokenIdEntityFrameworkValueConverter : ValueConverter<TokenId, string>
{
    public TokenIdEntityFrameworkValueConverter() : this(null) { }

    public TokenIdEntityFrameworkValueConverter(ConverterMappingHints mappingHints)
        : base(
            id => id == null ? null : id.StringValue,
            value => TokenId.Parse(value),
            mappingHints
        )
    { }
}
