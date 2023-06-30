using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.ValueConverters;
public class PnsHandleEntityFrameworkValueConverter : ValueConverter<PnsHandle, string>
{
    public PnsHandleEntityFrameworkValueConverter() : this(null)
    {
    }

    public PnsHandleEntityFrameworkValueConverter(ConverterMappingHints? mappingHints)
        : base(
            pnsHandle => $"{Enum.GetName(typeof(PushNotificationPlatform),pnsHandle.Platform)}|{pnsHandle.Value}",
            value => PnsHandleStringParse(value),
            mappingHints
        )
    {
    }

    private static PnsHandle PnsHandleStringParse(string pnsHandle)
    {
        var tokens = pnsHandle.Split('|');
        var platform = (PushNotificationPlatform)Enum.Parse(typeof(PushNotificationPlatform), tokens[0]);
        var value = tokens[1];
        return PnsHandle.Parse(value, platform).Value;
    }
}
