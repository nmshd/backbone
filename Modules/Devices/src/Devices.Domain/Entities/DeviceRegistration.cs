namespace Backbone.Modules.Devices.Domain.Entities;

public class DeviceRegistration
{
#pragma warning disable CS8618
    private DeviceRegistration() { }
#pragma warning restore CS8618

    public DeviceRegistration(string platform, string handle, string installationId, string appId)
    {
        Platform = platform;
        Handle = handle;
        InstallationId = installationId;
        AppId = appId;
    }

    public string Platform { get; }
    public string Handle { get; }
    public string InstallationId { get; }
    public string AppId { get; }
}
