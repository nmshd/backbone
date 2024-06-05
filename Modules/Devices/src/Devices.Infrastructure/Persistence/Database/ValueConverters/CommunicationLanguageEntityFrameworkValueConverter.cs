using Backbone.Modules.Devices.Domain.Entities.Identities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.ValueConverters;

public class CommunicationLanguageEntityFrameworkValueConverter : ValueConverter<CommunicationLanguage, string>
{
    public CommunicationLanguageEntityFrameworkValueConverter() : this(null)
    {
    }

    public CommunicationLanguageEntityFrameworkValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id.Value,
            value => CommunicationLanguage.Create(value).Value,
            mappingHints
        )
    {
    }
}

public class NullableCommunicationLanguageValueConverter : ValueConverter<CommunicationLanguage?, string?>
{
    public NullableCommunicationLanguageValueConverter() : this(null)
    {
    }

    public NullableCommunicationLanguageValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id == null ? null : id.Value,
            value => value == null ? null : CommunicationLanguage.Create(value).Value,
            mappingHints
        )
    {
    }
}
