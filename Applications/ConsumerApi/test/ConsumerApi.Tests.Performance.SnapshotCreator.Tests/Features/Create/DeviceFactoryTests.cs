using System.Reflection;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Helper;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Features.Create;

public class DeviceFactoryTests
{
    private Client? _sdkClient;


    private Client? GetSdkClient()
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

        var deviceData = new DeviceData { DeviceId = "deviceId", UserCredentials = new UserCredentials("username", "password") };
        var identityData = new IdentityData { Address = "address", KeyPair = null! };

        return _sdkClient = (Client?)Activator.CreateInstance(
            typeof(Client),
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            [httpClient, configuration, deviceData, identityData],
            null);
    }


    [Fact]
    public async Task CreateDevices_NumDeviceIdsIsOne_ReturnsEmptyList()
    {
        // ARRANGE
        _sdkClient ??= GetSdkClient();
        var logger = A.Fake<ILogger<DeviceFactory>>();
        var consumerApiClient = A.Fake<IConsumerApiHelper>();

        A.CallTo(() => consumerApiClient.CreateForExistingIdentity(A<CreateDevices.Command>.Ignored, A<DomainIdentity>.Ignored))!.Returns(_sdkClient);
        A.CallTo(() => consumerApiClient.OnBoardNewDevice(A<DomainIdentity>.Ignored, A<Client>.Ignored))
            .Returns($"deviceId");

        var request = A.Fake<CreateDevices.Command>();
        var identity = A.Fake<DomainIdentity>() with { NumberOfDevices = 1 };

        var sut = new DeviceFactory(logger, consumerApiClient);

        // ACT
        var result = await sut.CreateDevices(request, identity);

        // ASSERT
        result.Should().BeEmpty();
        identity.DeviceIds.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateDevices_NumDeviceIdsGreaterOne_ReturnsDeviceId()
    {
        // ARRANGE
        _sdkClient ??= GetSdkClient();

        var logger = A.Fake<ILogger<DeviceFactory>>();

        var consumerApiClient = A.Fake<IConsumerApiHelper>();

        A.CallTo(() => consumerApiClient.CreateForExistingIdentity(A<CreateDevices.Command>.Ignored, A<DomainIdentity>.Ignored))!.Returns(_sdkClient);

        A.CallTo(() => consumerApiClient.OnBoardNewDevice(A<DomainIdentity>.Ignored, A<Client>.Ignored))
            .Returns($"deviceId");

        var request = A.Fake<CreateDevices.Command>();
        var identity = A.Fake<DomainIdentity>() with { NumberOfDevices = 5 };

        var sut = new DeviceFactory(logger, consumerApiClient);

        // ACT
        var result = await sut.CreateDevices(request, identity);

        // ASSERT

        A.CallTo(() => consumerApiClient.CreateForExistingIdentity(request, identity))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => consumerApiClient.OnBoardNewDevice(identity, _sdkClient!))
            .MustHaveHappened(identity.NumberOfDevices - 1, Times.Exactly);

        result.Should().HaveCount(identity.NumberOfDevices - 1);
        identity.DeviceIds.Should().BeEquivalentTo(result);
    }


    [Fact]
    public async Task Create_NumDeviceIdsGreaterOne_NumberOfCreatedDevicesShouldBeEqualToIdentityDevices()
    {
        // ARRANGE
        _sdkClient ??= GetSdkClient();
        var logger = A.Fake<ILogger<DeviceFactory>>();
        var consumerApiClient = A.Fake<IConsumerApiHelper>();

        A.CallTo(() => consumerApiClient.CreateForExistingIdentity(A<CreateDevices.Command>.Ignored, A<DomainIdentity>.Ignored))!.Returns(_sdkClient);
        A.CallTo(() => consumerApiClient.OnBoardNewDevice(A<DomainIdentity>.Ignored, A<Client>.Ignored))
            .Returns($"deviceId");

        var request = A.Fake<CreateDevices.Command>();
        var identity = A.Fake<DomainIdentity>() with { NumberOfDevices = 5 };
        var sut = new DeviceFactory(logger, consumerApiClient);

        // ACT
        await sut.Create(request, identity);

        // ASSERT

        sut.NumberOfCreatedDevices.Should().Be(identity.NumberOfDevices - 1);
        identity.DeviceIds.Should().HaveCount(identity.NumberOfDevices - 1);
    }

    [Fact]
    public async Task Create_AfterInvoked_ShouldReleaseSemaphoreSlim()
    {
        // ARRANGE
        _sdkClient ??= GetSdkClient();
        var logger = A.Fake<ILogger<DeviceFactory>>();
        var consumerApiClient = A.Fake<IConsumerApiHelper>();

        A.CallTo(() => consumerApiClient.CreateForExistingIdentity(A<CreateDevices.Command>.Ignored, A<DomainIdentity>.Ignored))!.Returns(_sdkClient);
        A.CallTo(() => consumerApiClient.OnBoardNewDevice(A<DomainIdentity>.Ignored, A<Client>.Ignored))
            .Returns($"deviceId");

        var request = A.Fake<CreateDevices.Command>();
        var identity = A.Fake<DomainIdentity>() with { NumberOfDevices = 5 };

        var sut = new DeviceFactory(logger, consumerApiClient);

        // ACT
        await sut.Create(request, identity);

        // ASSERT
        sut._semaphoreSlim.CurrentCount.Should().Be(Environment.ProcessorCount);
    }
}
