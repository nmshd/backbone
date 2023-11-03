using Backbone.Modules.Devices.Application.Tests.Tests.PushNotifications;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.ValueConverters;
internal class DevicePushIdentifierEntityFrameworkValueConverter : ValueConverter<DevicePushIdentifier, string>
{
    public DevicePushIdentifierEntityFrameworkValueConverter() : this(null) { }

    public DevicePushIdentifierEntityFrameworkValueConverter(ConverterMappingHints? mappingHints)
        : base(
            dpi => dpi.StringValue,
            value => DevicePushIdentifier.Parse(value),
            mappingHints)
    { }
}
