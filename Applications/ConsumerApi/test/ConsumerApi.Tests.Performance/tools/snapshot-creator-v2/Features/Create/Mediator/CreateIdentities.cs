using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;
using Backbone.Tooling;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Mediator;

public record CreateIdentities
{
    public record Command(
        List<IdentityPoolConfiguration> IdentityPoolConfigurations,
        string BaseUrl,
        ClientCredentials ClientCrentials) : IRequest<List<DomainIdentity>>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 
    public record CommandHandler(ILogger<CommandHandler> Logger) : IRequestHandler<Command, List<DomainIdentity>>
    {
        public async Task<List<DomainIdentity>> Handle(Command request, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Creating identities ...");
            var identities = new List<DomainIdentity>();


            foreach (var identityPoolConfiguration in request.IdentityPoolConfigurations)
            {
                foreach (var identityConfiguration in identityPoolConfiguration.Identities)
                {
                    var sdkClient = await Client.CreateForNewIdentity(request.BaseUrl, request.ClientCrentials, PasswordHelper.GeneratePassword(18, 24));

                    if (sdkClient.DeviceData is null)
                        throw new Exception("The SDK could not be used to create a new database Identity.");

                    var deviceDataDeviceId = sdkClient.IdentityData?.Address ?? "no address";

                    var createdIdentity = new DomainIdentity(
                        sdkClient.DeviceData.UserCredentials,
                        deviceDataDeviceId,
                        sdkClient.DeviceData.DeviceId,
                        identityPoolConfiguration,
                        identityConfiguration.Address);

                    identities.Add(createdIdentity);

                    if (identityConfiguration.Devices == 0) continue;

                    for (var i = 0; i < identityConfiguration.Devices; i++)
                    {
                        var newDevice = await sdkClient.OnboardNewDevice(PasswordHelper.GeneratePassword(18, 24));

                        if (newDevice is null)
                            throw new Exception("The SDK could not be used to create a new database Device.");

                        if (newDevice.DeviceData is null)
                            throw new Exception($"The SDK could not be used to create a new database Device. {nameof(newDevice.DeviceData)} is null.");

                        createdIdentity.AddDevice(newDevice.DeviceData.DeviceId);
                    }
                }
            }

            return identities;
        }
    }
}
