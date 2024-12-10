using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Helper;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Features.Create.Factories;

public class IdentityFactoryTests : SnapshotCreatorTestsBase
{
    private Client? _sdkClient;
    private readonly IdentityFactory _sut;
    private readonly IConsumerApiHelper _consumerApiClient;
    private readonly ILogger<IdentityFactory> _logger;

    public IdentityFactoryTests()
    {
        _logger = A.Fake<ILogger<IdentityFactory>>();
        _consumerApiClient = A.Fake<IConsumerApiHelper>();

        _sut = new IdentityFactory(_logger, _consumerApiClient);
    }

    [Fact]
    public async Task Create_NumIdentitiesIsOne_ReturnsIdentity()
    {
        // ARRANGE
        _sdkClient = GetSdkClient();
        A.CallTo(() => _consumerApiClient.CreateForNewIdentity(A<CreateIdentities.Command>.Ignored))!.Returns(_sdkClient);

        var request = A.Fake<CreateIdentities.Command>();
        var identityConfiguration = A.Fake<IdentityConfiguration>();

        // ACT
        var result = await _sut.Create(request, identityConfiguration);

        // ASSERT
        result.Should().NotBeNull();
        result.DeviceIds.Count.Should().Be(1);
        _sut.TotalCreatedIdentities.Should().Be(result.DeviceIds.Count);
        _sut.GetSemaphoreCurrentCount().Should().Be(IdentityFactory.MaxDegreeOfParallelism);
    }

    [Fact]
    public async Task Create_AfterInvoked_ShouldReleaseSemaphoreSlim()
    {
        // ARRANGE
        _sdkClient = GetSdkClient();
        A.CallTo(() => _consumerApiClient.CreateForNewIdentity(A<CreateIdentities.Command>.Ignored))!.Returns(_sdkClient);

        var request = A.Fake<CreateIdentities.Command>();
        var identityConfiguration = A.Fake<IdentityConfiguration>();

        // ACT
        await _sut.Create(request, identityConfiguration);

        // ASSERT
        _sut.GetSemaphoreCurrentCount().Should().Be(IdentityFactory.MaxDegreeOfParallelism);
    }


    [Fact]
    public async Task InnerCreate_SdkClientDeviceDataNull_ShouldThrowException()
    {
        // ARRANGE
        _sdkClient = GetSdkClient(isDeviceDataSet: false);
        A.CallTo(() => _consumerApiClient.CreateForNewIdentity(A<CreateIdentities.Command>.Ignored))!.Returns(_sdkClient);

        var request = A.Fake<CreateIdentities.Command>();
        var identityConfiguration = A.Fake<IdentityConfiguration>();

        // ACT + ASSERT
        Func<Task<DomainIdentity>> act = async () => await _sut.InnerCreate(request, identityConfiguration);
        await act.Should().ThrowAsync<InvalidOperationException>();
        _sut.GetSemaphoreCurrentCount().Should().Be(IdentityFactory.MaxDegreeOfParallelism);
    }


    [Fact]
    public async Task InnerCreate_SdkClientDeviceDataDeviceIdNull_ShouldThrowException()
    {
        // ARRANGE
        _sdkClient = GetSdkClient(isDeviceDataDeviceIdSet: false);
        A.CallTo(() => _consumerApiClient.CreateForNewIdentity(A<CreateIdentities.Command>.Ignored))!.Returns(_sdkClient);

        var request = A.Fake<CreateIdentities.Command>();
        var identityConfiguration = A.Fake<IdentityConfiguration>();

        var sut = new IdentityFactory(_logger, _consumerApiClient);

        // ACT + ASSERT
        Func<Task<DomainIdentity>> act = async () => await sut.InnerCreate(request, identityConfiguration);
        await act.Should().ThrowAsync<InvalidOperationException>();
        _sut.GetSemaphoreCurrentCount().Should().Be(IdentityFactory.MaxDegreeOfParallelism);
    }
}
