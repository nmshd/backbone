using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;
using Backbone.Tooling;
using MediatR;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Mediator;

public record CreateIdentities
{
    public record Command(
        List<IdentityPoolConfiguration> IdentityPoolConfigurations,
        string BaseUrlAddress,
        ClientCredentials ClientCredentials) : IRequest<List<DomainIdentity>>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 
    public record CommandHandler : IRequestHandler<Command, List<DomainIdentity>>
    {
        public async Task<List<DomainIdentity>> Handle(Command request, CancellationToken cancellationToken)
        {
            var identities = new List<DomainIdentity>();

            foreach (var identityPoolConfiguration in request.IdentityPoolConfigurations)
            {
                foreach (var identityConfiguration in identityPoolConfiguration.Identities)
                {
                    var sdkClient = await Client.CreateForNewIdentity(request.BaseUrlAddress, request.ClientCredentials, PasswordHelper.GeneratePassword(18, 24));

                    if (sdkClient.DeviceData is null)
                        throw new Exception("The SDK could not be used to create a new database Identity.");

                    var identityAddress = sdkClient.IdentityData?.Address ?? "no address";

                    var createdIdentity = new DomainIdentity(
                        sdkClient.DeviceData.UserCredentials,
                        identityAddress,
                        sdkClient.DeviceData.DeviceId,
                        identityPoolConfiguration,
                        identityConfiguration.Address,
                        identityConfiguration.NumberOfDevices,
                        identityConfiguration.NumberOfRelationshipTemplates,
                        identityConfiguration.IdentityPoolType);

                    identities.Add(createdIdentity);
                }
            }

            return identities;
        }
    }
}
