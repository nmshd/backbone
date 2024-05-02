using Backbone.Modules.Tokens.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Tokens.Infrastructure.Persistence.Database.ValueConverters;

public class TokenIdEntityFrameworkValueConverter : ValueConverter<TokenId, string>
{
    public TokenIdEntityFrameworkValueConverter() : this(null)
    {
    }

    public TokenIdEntityFrameworkValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id,
            value => TokenId.Parse(value),
            mappingHints
        )
    {
    }
}
