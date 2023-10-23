using Backbone.Messages.Domain.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Messages.Infrastructure.Persistence.Database.ValueConverters;

public class MessageIdEntityFrameworkValueConverter : ValueConverter<MessageId, string>
{
    public MessageIdEntityFrameworkValueConverter() : this(null) { }

    public MessageIdEntityFrameworkValueConverter(ConverterMappingHints mappingHints)
        : base(
            id => id == null ? null : id.StringValue,
            value => MessageId.Parse(value),
            mappingHints
        )
    { }
}
