using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Helper;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Enums;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Features.Create.Factories;

public class ChallengeFactoryTests : SnapshotCreatorTestsBase
{
    private readonly IConsumerApiHelper _consumerApiHelper;
    private readonly ChallengeFactory _sut;

    public ChallengeFactoryTests()
    {
        var logger = A.Fake<ILogger<ChallengeFactory>>();
        _consumerApiHelper = A.Fake<IConsumerApiHelper>();
        _sut = new ChallengeFactory(logger, _consumerApiHelper);
    }

    [Fact]
    public async Task Create_ShouldSetNumberOfCreatedChallenges()
    {
        // Arrange
        var identity = new DomainIdentity(null!, null, 0, 0, 0, IdentityPoolType.App, 5, "", 0, 0);
        var expectedNumberOfCreatedChallenges = identity.NumberOfChallenges;
        var command = new CreateChallenges.Command([identity], "http://baseurl", new ClientCredentials("clientId", "clientSecret"));
        var client = GetSdkClient();
        var challenge = new Challenge { Id = "challengeId", ExpiresAt = DateTime.UtcNow };

        A.CallTo(() => _consumerApiHelper.CreateForExistingIdentity(A<string>.Ignored, A<ClientCredentials>.Ignored, A<UserCredentials>.Ignored, null))!.Returns(client);
        A.CallTo(() => _consumerApiHelper.CreateChallenge(client!)).Returns(new ApiResponse<Challenge> { Result = challenge });
        // Act
        await _sut.Create(command, identity);

        // Assert
        _sut.TotalCreatedChallenges.Should().Be(expectedNumberOfCreatedChallenges);
        _sut.GetSemaphoreCurrentCount().Should().Be(Environment.ProcessorCount);
    }


    [Fact]
    public async Task CreateChallenges_ShouldReturnChallenges()
    {
        // Arrange
        var identity = new DomainIdentity(null!, null, 0, 0, 0, IdentityPoolType.App, 5, "", 0, 0);
        var expectedNumberOfCreatedChallenges = identity.NumberOfChallenges;
        var command = new CreateChallenges.Command([identity], "http://baseurl", new ClientCredentials("clientId", "clientSecret"));
        var client = GetSdkClient();
        var challenge = new Challenge { Id = "challengeId", ExpiresAt = DateTime.UtcNow };

        A.CallTo(() => _consumerApiHelper.CreateForExistingIdentity(command.BaseUrlAddress, command.ClientCredentials, identity.UserCredentials, null))!.Returns(client);
        A.CallTo(() => _consumerApiHelper.CreateChallenge(client!)).Returns(new ApiResponse<Challenge> { Result = challenge });

        // Act
        var result = await _sut.CreateChallenges(command, identity);

        // Assert
        result.Count.Should().Be(expectedNumberOfCreatedChallenges);
        _sut.GetSemaphoreCurrentCount().Should().Be(Environment.ProcessorCount);
    }


    [Fact]
    public async Task CreateChallenges_ShouldLogWarningOnDeviceIdNull()
    {
        // Arrange
        var client = GetSdkClient(isDeviceDataDeviceIdSet: false);
        var identity = new DomainIdentity(null!, null, 0, 0, 0, IdentityPoolType.App, 5, "", 0, 0);
        var expectedNumberOfCreatedChallenges = identity.NumberOfChallenges;

        var command = new CreateChallenges.Command([identity], "http://baseurl", new ClientCredentials("clientId", "clientSecret"));
        var challenge = new Challenge { Id = "challengeId", ExpiresAt = DateTime.UtcNow };

        A.CallTo(() => _consumerApiHelper.CreateForExistingIdentity(command.BaseUrlAddress, command.ClientCredentials, identity.UserCredentials, null))!.Returns(client);
        A.CallTo(() => _consumerApiHelper.CreateChallenge(client!)).Returns(new ApiResponse<Challenge> { Result = challenge });

        // Act
        var result = await _sut.CreateChallenges(command, identity);

        // Assert
        result.Count.Should().Be(expectedNumberOfCreatedChallenges);
        _sut.GetSemaphoreCurrentCount().Should().Be(ChallengeFactory.MaxDegreeOfParallelism);
    }


    [Fact]
    public async Task CreateChallenges_ShouldThrowExceptionWhenApiResponseIsError()
    {
        // Arrange
        var identity = new DomainIdentity(null!, null, 0, 0, 0, IdentityPoolType.App, 5, "", 0, 0);
        var command = new CreateChallenges.Command([identity], "http://baseurl", new ClientCredentials("clientId", "clientSecret"));
        var client = GetSdkClient();

        A.CallTo(() => _consumerApiHelper.CreateForExistingIdentity(command.BaseUrlAddress, command.ClientCredentials, identity.UserCredentials, null))!.Returns(client);

        var apiResponse = new ApiResponse<Challenge>
        {
            Error = A.Fake<ApiError>()
        };

        A.CallTo(() => _consumerApiHelper.CreateChallenge(client!)).Returns(apiResponse);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateChallenges(command, identity));
        _sut.GetSemaphoreCurrentCount().Should().Be(ChallengeFactory.MaxDegreeOfParallelism);
    }
}
