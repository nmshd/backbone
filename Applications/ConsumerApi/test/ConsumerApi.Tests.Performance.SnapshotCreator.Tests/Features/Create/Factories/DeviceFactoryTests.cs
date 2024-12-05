using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Helper;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Features.Create.Factories;

public class DeviceFactoryTests : SnapshotCreatorTestsBase
{
    private Client? _sdkClient;

    [Fact]
    public async Task CreateDevices_NumDeviceIdsIsOne_ShouldReturnEmptyList()
    {
        // ARRANGE
        _sdkClient ??= GetSdkClient();
        var logger = A.Fake<ILogger<DeviceFactory>>();
        var consumerApiClient = A.Fake<IConsumerApiHelper>();

        A.CallTo(() => consumerApiClient.CreateForExistingIdentity(A<string>.Ignored, A<ClientCredentials>.Ignored, A<UserCredentials>.Ignored, A<IdentityData>.Ignored))!.Returns(_sdkClient);
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
    public async Task CreateDevices_NumDeviceIdsGreaterOne_ShouldBeEqualToIdentityDevices()
    {
        // ARRANGE
        _sdkClient ??= GetSdkClient();

        var logger = A.Fake<ILogger<DeviceFactory>>();

        var consumerApiClient = A.Fake<IConsumerApiHelper>();

        A.CallTo(() => consumerApiClient.CreateForExistingIdentity(A<string>.Ignored, A<ClientCredentials>.Ignored, A<UserCredentials>.Ignored, A<IdentityData>.Ignored))!.Returns(_sdkClient);

        A.CallTo(() => consumerApiClient.OnBoardNewDevice(A<DomainIdentity>.Ignored, A<Client>.Ignored))
            .Returns($"deviceId");

        var request = A.Fake<CreateDevices.Command>();
        var identity = A.Fake<DomainIdentity>() with { NumberOfDevices = 5 };

        var sut = new DeviceFactory(logger, consumerApiClient);

        // ACT
        var result = await sut.CreateDevices(request, identity);

        // ASSERT
        A.CallTo(() => consumerApiClient.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, identity.UserCredentials, null))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => consumerApiClient.OnBoardNewDevice(identity, _sdkClient!))
            .MustHaveHappened(identity.NumberOfDevices - 1, Times.Exactly);

        result.Should().HaveCount(identity.NumberOfDevices - 1);
        identity.DeviceIds.Should().BeEquivalentTo(result);
    }


    [Fact]
    public async Task Create_NumDeviceIdsGreaterOne_ShouldBeEqualToIdentityDevices()
    {
        // ARRANGE
        _sdkClient ??= GetSdkClient();
        var logger = A.Fake<ILogger<DeviceFactory>>();
        var consumerApiClient = A.Fake<IConsumerApiHelper>();

        A.CallTo(() => consumerApiClient.CreateForExistingIdentity(A<string>.Ignored, A<ClientCredentials>.Ignored, A<UserCredentials>.Ignored, A<IdentityData>.Ignored))!.Returns(_sdkClient);
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

        A.CallTo(() => consumerApiClient.CreateForExistingIdentity(A<string>.Ignored, A<ClientCredentials>.Ignored, A<UserCredentials>.Ignored, A<IdentityData>.Ignored))!.Returns(_sdkClient);
        A.CallTo(() => consumerApiClient.OnBoardNewDevice(A<DomainIdentity>.Ignored, A<Client>.Ignored))
            .Returns($"deviceId");

        var request = A.Fake<CreateDevices.Command>();
        var identity = A.Fake<DomainIdentity>() with { NumberOfDevices = 5 };

        var sut = new DeviceFactory(logger, consumerApiClient);

        // ACT
        await sut.Create(request, identity);

        // ASSERT
        sut.SemaphoreSlim.CurrentCount.Should().Be(Environment.ProcessorCount);
    }
}
