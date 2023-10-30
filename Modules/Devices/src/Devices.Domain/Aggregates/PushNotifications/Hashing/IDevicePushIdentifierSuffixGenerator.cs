namespace Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Hashing;

public interface IDevicePushIdentifierSuffixGenerator
{
    string GenerateSuffixUtf8(string seed);
}
