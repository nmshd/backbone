using System.Reflection;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Readers;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;

public abstract class SnapshotCreatorTestsBase : AbstractTestsBase
{
    protected SnapshotCreatorTestsBase()
    {
        AssertionOptions.AssertEquivalencyUsing(options => options.ComparingRecordsByValue());
    }

    protected string TestDataFolder { get; } = Path.Combine(AppContext.BaseDirectory, "TestData");

    protected async Task<PerformanceTestConfiguration?> GetExpectedPoolConfiguration(string expectedPoolConfigJsonFilename)
    {
        var fullFilePath = Path.Combine(TestDataFolder, expectedPoolConfigJsonFilename);
        var result = await new PoolConfigurationJsonReader().Read(fullFilePath);

        return result;
    }

    protected Client? GetSdkClient(bool isDeviceDataSet = true, bool isDeviceDataDeviceIdSet = true)
    {
        var httpClient = new HttpClient();
        var configuration = new Configuration
        {
            Authentication = new Configuration.AuthenticationConfiguration
            {
                ClientCredentials = new ClientCredentials("test", "test"),
                UserCredentials = new UserCredentials("username", "password")
            }
        };
        var deviceData = isDeviceDataSet
            ? isDeviceDataDeviceIdSet
                ? new DeviceData { DeviceId = "deviceId", UserCredentials = new UserCredentials("username", "password") }
                : new DeviceData { DeviceId = null!, UserCredentials = new UserCredentials("username", "password") }
            : null;
        var identityData = new IdentityData { Address = "address", KeyPair = null! };
        return (Client?)Activator.CreateInstance(
            typeof(Client),
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            [httpClient, configuration, deviceData, identityData],
            null);
    }
}
