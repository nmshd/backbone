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
    private readonly Client? _sdkClient;
    private readonly IConsumerApiHelper _consumerApiClient;
    private readonly DeviceFactory _sut;

    public DeviceFactoryTests()
    {
        _sdkClient = GetSdkClient();
        var logger = A.Fake<ILogger<DeviceFactory>>();
        _consumerApiClient = A.Fake<IConsumerApiHelper>();

        _sut = new DeviceFactory(logger, _consumerApiClient);
    }

    [Fact]
    public async Task CreateDevices_NumDeviceIdsIsOne_ShouldReturnEmptyList()
    {
        // ARRANGE
        A.CallTo(() => _consumerApiClient.CreateForExistingIdentity(A<string>.Ignored, A<ClientCredentials>.Ignored, A<UserCredentials>.Ignored, A<IdentityData>.Ignored))!.Returns(_sdkClient);
        A.CallTo(() => _consumerApiClient.OnBoardNewDevice(A<DomainIdentity>.Ignored, A<Client>.Ignored))
            .Returns($"deviceId");

        var request = A.Fake<CreateDevices.Command>();
        var identity = A.Fake<DomainIdentity>() with { NumberOfDevices = 1 };

        // ACT
        var result = await _sut.CreateDevices(request, identity);

        // ASSERT
        result.Should().BeEmpty();
        identity.DeviceIds.Should().BeEmpty();
        _sut.GetSemaphoreCurrentCount().Should().Be(Environment.ProcessorCount);
    }

    [Fact]
    public async Task CreateDevices_NumDeviceIdsGreaterOne_ShouldBeEqualToIdentityDevices()
    {
        // ARRANGE
        A.CallTo(() => _consumerApiClient.CreateForExistingIdentity(A<string>.Ignored, A<ClientCredentials>.Ignored, A<UserCredentials>.Ignored, A<IdentityData>.Ignored))!.Returns(_sdkClient);
        A.CallTo(() => _consumerApiClient.OnBoardNewDevice(A<DomainIdentity>.Ignored, A<Client>.Ignored))
            .Returns($"deviceId");

        var request = A.Fake<CreateDevices.Command>();
        var identity = A.Fake<DomainIdentity>() with { NumberOfDevices = 5 };

        // ACT
        var result = await _sut.CreateDevices(request, identity);

        // ASSERT
        A.CallTo(() => _consumerApiClient.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, identity.UserCredentials, identity.IdentityData))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _consumerApiClient.OnBoardNewDevice(identity, _sdkClient!))
            .MustHaveHappened(identity.NumberOfDevices - 1, Times.Exactly);

        result.Should().HaveCount(identity.NumberOfDevices - 1);
        identity.DeviceIds.Should().BeEquivalentTo(result);
    }


    [Fact]
    public async Task Create_NumDeviceIdsGreaterOne_ShouldBeEqualToIdentityDevices()
    {
        // ARRANGE
        A.CallTo(() => _consumerApiClient.CreateForExistingIdentity(A<string>.Ignored, A<ClientCredentials>.Ignored, A<UserCredentials>.Ignored, A<IdentityData>.Ignored))!.Returns(_sdkClient);
        A.CallTo(() => _consumerApiClient.OnBoardNewDevice(A<DomainIdentity>.Ignored, A<Client>.Ignored))
            .Returns($"deviceId");

        var request = A.Fake<CreateDevices.Command>();
        var identity = A.Fake<DomainIdentity>() with { NumberOfDevices = 5 };

        // ACT
        await _sut.Create(request, identity);

        // ASSERT

        _sut.TotalCreatedDevices.Should().Be(identity.NumberOfDevices - 1);
        identity.DeviceIds.Should().HaveCount(identity.NumberOfDevices - 1);
        _sut.GetSemaphoreCurrentCount().Should().Be(Environment.ProcessorCount);
    }
}
