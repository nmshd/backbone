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

    [Fact]
    public async Task Create_NumIdentitiesIsOne_ReturnsIdentity()
    {
        // ARRANGE
        _sdkClient ??= GetSdkClient();
        var logger = A.Fake<ILogger<IdentityFactory>>();
        var consumerApiClient = A.Fake<IConsumerApiHelper>();
        A.CallTo(() => consumerApiClient.CreateForNewIdentity(A<CreateIdentities.Command>.Ignored))!.Returns(_sdkClient);

        var request = A.Fake<CreateIdentities.Command>();
        var identityConfiguration = A.Fake<IdentityConfiguration>();

        var sut = new IdentityFactory(logger, consumerApiClient);

        // ACT
        var result = await sut.Create(request, identityConfiguration);

        // ASSERT
        result.Should().NotBeNull();
        result.DeviceIds.Count.Should().Be(1);
        sut.NumberOfCreatedIdentities.Should().Be(result.DeviceIds.Count);
    }

    [Fact]
    public async Task Create_AfterInvoked_ShouldReleaseSemaphoreSlim()
    {
        // ARRANGE
        _sdkClient ??= GetSdkClient();
        var logger = A.Fake<ILogger<IdentityFactory>>();
        var consumerApiClient = A.Fake<IConsumerApiHelper>();
        A.CallTo(() => consumerApiClient.CreateForNewIdentity(A<CreateIdentities.Command>.Ignored))!.Returns(_sdkClient);

        var request = A.Fake<CreateIdentities.Command>();
        var identityConfiguration = A.Fake<IdentityConfiguration>();

        var sut = new IdentityFactory(logger, consumerApiClient);

        // ACT
        await sut.Create(request, identityConfiguration);

        // ASSERT
        sut.SemaphoreSlim.CurrentCount.Should().Be(Environment.ProcessorCount);
    }


    [Fact]
    public async Task InnerCreate_SdkClientDeviceDataNull_ShouldThrowException()
    {
        // ARRANGE
        _sdkClient ??= GetSdkClient(isDeviceDataSet: false);

        var logger = A.Fake<ILogger<IdentityFactory>>();
        var consumerApiClient = A.Fake<IConsumerApiHelper>();
        A.CallTo(() => consumerApiClient.CreateForNewIdentity(A<CreateIdentities.Command>.Ignored))!.Returns(_sdkClient);

        var request = A.Fake<CreateIdentities.Command>();
        var identityConfiguration = A.Fake<IdentityConfiguration>();

        var sut = new IdentityFactory(logger, consumerApiClient);

        // ACT + ASSERT
        Func<Task<DomainIdentity>> act = async () => await sut.InnerCreate(request, identityConfiguration);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }


    [Fact]
    public async Task InnerCreate_SdkClientDeviceDataDeviceIdNull_ShouldThrowException()
    {
        // ARRANGE
        _sdkClient ??= GetSdkClient(isDeviceDataDeviceIdSet: false);

        var logger = A.Fake<ILogger<IdentityFactory>>();
        var consumerApiClient = A.Fake<IConsumerApiHelper>();
        A.CallTo(() => consumerApiClient.CreateForNewIdentity(A<CreateIdentities.Command>.Ignored))!.Returns(_sdkClient);

        var request = A.Fake<CreateIdentities.Command>();
        var identityConfiguration = A.Fake<IdentityConfiguration>();

        var sut = new IdentityFactory(logger, consumerApiClient);

        // ACT + ASSERT
        Func<Task<DomainIdentity>> act = async () => await sut.InnerCreate(request, identityConfiguration);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
