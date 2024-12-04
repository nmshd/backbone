using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Backbone.Tooling;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public class ConsumerApiClient : IConsumerApiClient
{
    public Client CreateForExistingIdentity(CreateDevices.Command request, DomainIdentity identity)
    {
        var sdkClient = Client.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, identity.UserCredentials, identity.IdentityData);
        return sdkClient;
    }

    public async Task<string> OnBoardNewDevice(DomainIdentity identity, Client sdkClient)
    {
        var newDevice = await sdkClient.OnboardNewDevice(PasswordHelper.GeneratePassword(18, 24));

        return newDevice.DeviceData is null
            ? throw new InvalidOperationException(
                $"The SDK could not be used to create a new database Device for config {identity.IdentityAddress}/{identity.ConfigurationIdentityAddress}/{identity.PoolAlias} {IDENTITY_LOG_SUFFIX}")
            : newDevice.DeviceData.DeviceId;
    }
}
