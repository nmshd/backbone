namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.ApplePushNotificationService;

public class ApnsJwtCache
{
    private readonly Dictionary<string, Jwt> _jwts = new();
    public bool HasValue(string bundleId) => _jwts.GetValueOrDefault(bundleId) != null && !_jwts[bundleId].IsExpired();
    public Jwt GetValue(string bundleId) => _jwts[bundleId];
    public void UpdateValue(string bundleId, Jwt jwt) => _jwts[bundleId] = jwt;
}
