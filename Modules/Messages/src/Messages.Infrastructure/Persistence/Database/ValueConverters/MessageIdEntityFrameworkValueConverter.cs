using Backbone.Modules.Messages.Domain.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Messages.Infrastructure.Persistence.Database.ValueConverters;

public class MessageIdEntityFrameworkValueConverter : ValueConverter<MessageId, string>
{
    public MessageIdEntityFrameworkValueConverter() : this(null)
    {
    }

    public MessageIdEntityFrameworkValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id.Value,
            value => MessageId.Parse(value),
            mappingHints
        )
    {
    }
}
