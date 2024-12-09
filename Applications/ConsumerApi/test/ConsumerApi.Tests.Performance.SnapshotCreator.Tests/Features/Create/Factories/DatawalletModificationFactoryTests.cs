using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types.Responses;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Helper;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Enums;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Features.Create.Factories
{
    public class DatawalletModificationFactoryTests : SnapshotCreatorTestsBase
    {
        private readonly DatawalletModificationFactory _sut;
        private readonly IConsumerApiHelper _consumerApiHelper;

        public DatawalletModificationFactoryTests()
        {
            var logger = A.Fake<ILogger<DatawalletModificationFactory>>();
            _consumerApiHelper = A.Fake<IConsumerApiHelper>();
            _sut = new DatawalletModificationFactory(logger, _consumerApiHelper);
        }

        [Fact]
        public async Task Create_ShouldIncreaseNumberOfCreatedDatawalletModifications()
        {
            // Arrange
            var request = new CreateDatawalletModifications.Command(
                [new DomainIdentity(null!, null, 0, 0, 0, IdentityPoolType.Never, 5, "", 2, 0)],
                "http://baseurl",
                new ClientCredentials("clientId", "clientSecret")
            );

            var identity = request.Identities.First();
            var response = new ApiResponse<FinalizeDatawalletVersionUpgradeResponse>
            {
                Result = new FinalizeDatawalletVersionUpgradeResponse
                {
                    DatawalletModifications =
                    [
                        new CreatedDatawalletModification()
                        {
                            Id = "null",
                            Index = 0,
                            CreatedAt = default
                        }
                    ]
                }
            };

            var sdkClient = GetSdkClient();
            A.CallTo(() => _consumerApiHelper.CreateForExistingIdentity(A<string>._, A<ClientCredentials>._, A<UserCredentials>._, null))!
                .Returns(sdkClient);
            A.CallTo(() => _consumerApiHelper.StartSyncRun(sdkClient!))
                .Returns(new ApiResponse<StartSyncRunResponse>()
                {
                    Result = new StartSyncRunResponse
                    {
                        Status = "null",
                        SyncRun = null!
                    }
                });
            A.CallTo(() => _consumerApiHelper.FinalizeDatawalletVersionUpgrade(A<DomainIdentity>._, sdkClient!, A<ApiResponse<StartSyncRunResponse>>._))
                .Returns(response);

            // Act
            await _sut.Create(request, identity);

            // Assert
            _sut.TotalCreatedDatawalletModifications.Should().Be(1);
            _sut.GetSemaphoreCurrentCount().Should().Be(Environment.ProcessorCount);
        }


        [Fact]
        public async Task Create_ShouldThrowException_WhenFinalizeDatawalletVersionUpgradeResponseIsError()
        {
            // Arrange
            var request = new CreateDatawalletModifications.Command(
                [new DomainIdentity(null!, null, 0, 0, 0, IdentityPoolType.Never, 5, "", 2, 0)],
                "http://baseurl",
                new ClientCredentials("clientId", "clientSecret")
            );

            var identity = request.Identities.First();
            var response = new ApiResponse<FinalizeDatawalletVersionUpgradeResponse>();

            var sdkClient = GetSdkClient();
            A.CallTo(() => _consumerApiHelper.CreateForExistingIdentity(A<string>._, A<ClientCredentials>._, A<UserCredentials>._, null))!
                .Returns(sdkClient);
            A.CallTo(() => _consumerApiHelper.StartSyncRun(sdkClient!))
                .Returns(new ApiResponse<StartSyncRunResponse>());
            A.CallTo(() => _consumerApiHelper.FinalizeDatawalletVersionUpgrade(A<DomainIdentity>._, sdkClient!, A<ApiResponse<StartSyncRunResponse>>._))
                .Returns(response);

            // Act
            var act = async () => await _sut.Create(request, identity);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
            _sut.GetSemaphoreCurrentCount().Should().Be(Environment.ProcessorCount);
        }

        [Fact]
        public async Task Create_ShouldThrowException_WhenFinalizeDatawalletVersionUpgradeResponseIsNull()
        {
            // Arrange
            var request = new CreateDatawalletModifications.Command(
                [new DomainIdentity(null!, null, 0, 0, 0, IdentityPoolType.Never, 5, "", 2, 0)],
                "http://baseurl",
                new ClientCredentials("clientId", "clientSecret")
            );

            var identity = request.Identities.First();
            var response = new ApiResponse<FinalizeDatawalletVersionUpgradeResponse>() { Result = null };

            var sdkClient = GetSdkClient();
            A.CallTo(() => _consumerApiHelper.CreateForExistingIdentity(A<string>._, A<ClientCredentials>._, A<UserCredentials>._, null))!
                .Returns(sdkClient);
            A.CallTo(() => _consumerApiHelper.StartSyncRun(sdkClient!))
                .Returns(new ApiResponse<StartSyncRunResponse>()
                {
                    Result = new()
                    {
                        Status = "null",
                        SyncRun = null!
                    }
                });
            A.CallTo(() => _consumerApiHelper.FinalizeDatawalletVersionUpgrade(A<DomainIdentity>._, sdkClient!, A<ApiResponse<StartSyncRunResponse>>._))
                .Returns(response);

            // Act
            var act = async () => await _sut.Create(request, identity);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
            _sut.GetSemaphoreCurrentCount().Should().Be(Environment.ProcessorCount);
        }


        [Fact]
        public async Task Create_ShouldThrowException_WhenStartDatawalletVersionUpgradeResponseIsError()
        {
            // Arrange
            var request = new CreateDatawalletModifications.Command(
                [new DomainIdentity(null!, null, 0, 0, 0, IdentityPoolType.Never, 5, "", 2, 0)],
                "http://baseurl",
                new ClientCredentials("clientId", "clientSecret")
            );

            var identity = request.Identities.First();
            var response = new ApiResponse<FinalizeDatawalletVersionUpgradeResponse>() { Result = null };

            var sdkClient = GetSdkClient();
            A.CallTo(() => _consumerApiHelper.CreateForExistingIdentity(A<string>._, A<ClientCredentials>._, A<UserCredentials>._, null))!
                .Returns(sdkClient);
            A.CallTo(() => _consumerApiHelper.StartSyncRun(sdkClient!))
                .Returns(new ApiResponse<StartSyncRunResponse>()
                {
                    Error = new ApiError
                    {
                        Id = "null",
                        Code = "null",
                        Message = "null",
                        Docs = "null",
                        Time = default
                    }
                });
            A.CallTo(() => _consumerApiHelper.FinalizeDatawalletVersionUpgrade(A<DomainIdentity>._, sdkClient!, A<ApiResponse<StartSyncRunResponse>>._))
                .Returns(response);

            // Act
            var act = async () => await _sut.Create(request, identity);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
            _sut.GetSemaphoreCurrentCount().Should().Be(Environment.ProcessorCount);
        }
    }
}
