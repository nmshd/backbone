using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tokens.Domain.Entities;

namespace Tokens.Infrastructure.Persistence.Database.ValueConverters;

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
