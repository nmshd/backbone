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
        [Fact]
        public async Task Create_ShouldIncreaseNumberOfCreatedDatawalletModifications()
        {
            // Arrange
            var logger = A.Fake<ILogger<DatawalletModificationFactory>>();
            var consumerApiHelper = A.Fake<IConsumerApiHelper>();
            var factory = new DatawalletModificationFactory(logger, consumerApiHelper);

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
            A.CallTo(() => consumerApiHelper.CreateForExistingIdentity(A<string>._, A<ClientCredentials>._, A<UserCredentials>._, null))!
                .Returns(sdkClient);
            A.CallTo(() => consumerApiHelper.StartSyncRun(sdkClient!))
                .Returns(new ApiResponse<StartSyncRunResponse>()
                {
                    Result = new StartSyncRunResponse
                    {
                        Status = "null",
                        SyncRun = null!
                    }
                });
            A.CallTo(() => consumerApiHelper.FinalizeDatawalletVersionUpgrade(A<DomainIdentity>._, sdkClient!, A<ApiResponse<StartSyncRunResponse>>._))
                .Returns(response);

            // Act
            await factory.Create(request, identity);

            // Assert
            factory.NumberOfCreatedDatawalletModifications.Should().Be(1);
        }


        [Fact]
        public async Task Create_ShouldThrowException_WhenFinalizeDatawalletVersionUpgradeResponseIsError()
        {
            // Arrange
            var logger = A.Fake<ILogger<DatawalletModificationFactory>>();
            var consumerApiHelper = A.Fake<IConsumerApiHelper>();
            var factory = new DatawalletModificationFactory(logger, consumerApiHelper);

            var request = new CreateDatawalletModifications.Command(
                [new DomainIdentity(null!, null, 0, 0, 0, IdentityPoolType.Never, 5, "", 2, 0)],
                "http://baseurl",
                new ClientCredentials("clientId", "clientSecret")
            );

            var identity = request.Identities.First();
            var response = new ApiResponse<FinalizeDatawalletVersionUpgradeResponse>();

            var sdkClient = GetSdkClient();
            A.CallTo(() => consumerApiHelper.CreateForExistingIdentity(A<string>._, A<ClientCredentials>._, A<UserCredentials>._, null))!
                .Returns(sdkClient);
            A.CallTo(() => consumerApiHelper.StartSyncRun(sdkClient!))
                .Returns(new ApiResponse<StartSyncRunResponse>());
            A.CallTo(() => consumerApiHelper.FinalizeDatawalletVersionUpgrade(A<DomainIdentity>._, sdkClient!, A<ApiResponse<StartSyncRunResponse>>._))
                .Returns(response);

            // Act
            var act = async () => await factory.Create(request, identity);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task Create_ShouldThrowException_WhenFinalizeDatawalletVersionUpgradeResponseIsNull()
        {
            // Arrange
            var logger = A.Fake<ILogger<DatawalletModificationFactory>>();
            var consumerApiHelper = A.Fake<IConsumerApiHelper>();
            var factory = new DatawalletModificationFactory(logger, consumerApiHelper);

            var request = new CreateDatawalletModifications.Command(
                [new DomainIdentity(null!, null, 0, 0, 0, IdentityPoolType.Never, 5, "", 2, 0)],
                "http://baseurl",
                new ClientCredentials("clientId", "clientSecret")
            );

            var identity = request.Identities.First();
            var response = new ApiResponse<FinalizeDatawalletVersionUpgradeResponse>() { Result = null };
            ;

            var sdkClient = GetSdkClient();
            A.CallTo(() => consumerApiHelper.CreateForExistingIdentity(A<string>._, A<ClientCredentials>._, A<UserCredentials>._, null))!
                .Returns(sdkClient);
            A.CallTo(() => consumerApiHelper.StartSyncRun(sdkClient!))
                .Returns(new ApiResponse<StartSyncRunResponse>()
                {
                    Result = new()
                    {
                        Status = "null",
                        SyncRun = null!
                    }
                });
            A.CallTo(() => consumerApiHelper.FinalizeDatawalletVersionUpgrade(A<DomainIdentity>._, sdkClient!, A<ApiResponse<StartSyncRunResponse>>._))
                .Returns(response);

            // Act
            var act = async () => await factory.Create(request, identity);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }


        [Fact]
        public async Task Create_ShouldThrowException_WhenStartDatawalletVersionUpgradeResponseIsError()
        {
            // Arrange
            var logger = A.Fake<ILogger<DatawalletModificationFactory>>();
            var consumerApiHelper = A.Fake<IConsumerApiHelper>();
            var factory = new DatawalletModificationFactory(logger, consumerApiHelper);

            var request = new CreateDatawalletModifications.Command(
                [new DomainIdentity(null!, null, 0, 0, 0, IdentityPoolType.Never, 5, "", 2, 0)],
                "http://baseurl",
                new ClientCredentials("clientId", "clientSecret")
            );

            var identity = request.Identities.First();
            var response = new ApiResponse<FinalizeDatawalletVersionUpgradeResponse>() { Result = null };
            ;

            var sdkClient = GetSdkClient();
            A.CallTo(() => consumerApiHelper.CreateForExistingIdentity(A<string>._, A<ClientCredentials>._, A<UserCredentials>._, null))!
                .Returns(sdkClient);
            A.CallTo(() => consumerApiHelper.StartSyncRun(sdkClient!))
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
            A.CallTo(() => consumerApiHelper.FinalizeDatawalletVersionUpgrade(A<DomainIdentity>._, sdkClient!, A<ApiResponse<StartSyncRunResponse>>._))
                .Returns(response);

            // Act
            var act = async () => await factory.Create(request, identity);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
