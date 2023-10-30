namespace Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Hashing;

public interface IDevicePushIdentifierHasher
{
    string HashUtf8(string seed);
}
