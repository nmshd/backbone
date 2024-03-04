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
            pnsHandle => SerializeHandle(pnsHandle),
            value => DeserializeHandle(value),
            mappingHints
        )
    {
    }

    public static string SerializeHandle(PnsHandle pnsHandle)
    {
        var platformAsString = pnsHandle.Platform switch
        {
            PushNotificationPlatform.Fcm => "fcm",
            PushNotificationPlatform.Apns => "apns",
            _ => throw new NotImplementedException($"The platform '{pnsHandle.Platform}' is invalid.")
        };

        return $"{platformAsString}|{pnsHandle.Value}";
    }

    public static PnsHandle DeserializeHandle(string pnsHandle)
    {
        var tokens = pnsHandle.Split('|');
        var platform = tokens[0] switch
        {
            "fcm" => PushNotificationPlatform.Fcm,
            "apns" => PushNotificationPlatform.Apns,
            _ => throw new NotImplementedException($"The platform '{tokens[0]}' is invalid.")
        };
        var value = tokens[1];

        return PnsHandle.Parse(platform, value).Value;
    }
}
