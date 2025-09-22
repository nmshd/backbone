using Backbone.AdminApi.Infrastructure.Persistence.Models.Devices;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database.Converters;

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

    private static string SerializeHandle(PnsHandle pnsHandle)
    {
        var platformAsString = pnsHandle.Platform switch
        {
            PushNotificationPlatform.Fcm => "fcm",
            PushNotificationPlatform.Apns => "apns",
            PushNotificationPlatform.Dummy => "dummy",
            PushNotificationPlatform.Sse => "sse",
            _ => throw new NotSupportedException($"The platform '{pnsHandle.Platform}' is invalid.")
        };

        return $"{platformAsString}|{pnsHandle.Value}";
    }

    private static PnsHandle DeserializeHandle(string pnsHandle)
    {
        var tokens = pnsHandle.Split('|');
        var platform = tokens[0] switch
        {
            "fcm" => PushNotificationPlatform.Fcm,
            "apns" => PushNotificationPlatform.Apns,
            "dummy" => PushNotificationPlatform.Dummy,
            "sse" => PushNotificationPlatform.Sse,
            _ => throw new NotSupportedException($"The platform '{tokens[0]}' is invalid.")
        };
        var value = tokens[1];

        return new PnsHandle(platform, value);
        ;
    }
}
