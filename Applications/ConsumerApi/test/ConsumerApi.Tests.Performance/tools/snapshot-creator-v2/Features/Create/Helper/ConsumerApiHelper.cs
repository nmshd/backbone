using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Backbone.Tooling;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Helper;

public class ConsumerApiHelper : IConsumerApiHelper
{
    public Task<Client> CreateForNewIdentity(CreateIdentities.Command request) =>
        Client.CreateForNewIdentity(request.BaseUrlAddress, request.ClientCredentials, PasswordHelper.GeneratePassword(18, 24));

    public Client CreateForExistingIdentity(string baseUrl, ClientCredentials clientCredentials, UserCredentials userCredentials, IdentityData? identityData = null) =>
        Client.CreateForExistingIdentity(baseUrl, clientCredentials, userCredentials, identityData);

    public async Task<string> OnBoardNewDevice(DomainIdentity identity, Client sdkClient)
    {
        var newDevice = await sdkClient.OnboardNewDevice(PasswordHelper.GeneratePassword(18, 24));

        return newDevice.DeviceData is null
            ? throw new InvalidOperationException(
                $"The SDK could not be used to create a new database Device for config {identity.IdentityAddress}/{identity.ConfigurationIdentityAddress}/{identity.PoolAlias} {IDENTITY_LOG_SUFFIX}")
            : newDevice.DeviceData.DeviceId;
    }

    public Task<ApiResponse<Challenge>> CreateChallenge(Client sdkClient) => sdkClient.Challenges.CreateChallenge();
}
