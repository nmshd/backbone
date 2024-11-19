using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Backbone.Tooling;
using MediatR;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

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
                        throw new InvalidOperationException(
                            $"The SDK could not be used to create a new database Identity for config {identityConfiguration.Address}/{identityConfiguration.PoolAlias} [IdentityAddress/Pool]");

                    var createdIdentity = new DomainIdentity(
                        sdkClient.DeviceData.UserCredentials,
                        sdkClient.IdentityData,
                        identityConfiguration.Address,
                        identityConfiguration.NumberOfDevices,
                        identityConfiguration.NumberOfRelationshipTemplates,
                        identityConfiguration.IdentityPoolType,
                        identityConfiguration.NumberOfChallenges,
                        identityConfiguration.PoolAlias,
                        identityConfiguration.NumberOfDatawalletModifications);

                    createdIdentity.AddDevice(sdkClient.DeviceData.DeviceId);

                    identities.Add(createdIdentity);
                }
            }

            return identities;
        }
    }
}
