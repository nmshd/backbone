using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Enums;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using MediatR;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public record CreateRelationships
{
    public record Command(List<DomainIdentity> Identities, List<RelationshipAndMessages> RelationshipAndMessages, string BaseUrlAddress, ClientCredentials ClientCredentials)
        : IRequest<Unit>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 
    public record CommandHandler : IRequestHandler<Command, Unit>
    {
        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var connectorIdentities = request.Identities.Where(i => i.IdentityPoolType == IdentityPoolType.Connector).ToList();
            var appIdentities = request.Identities.Where(i => i.IdentityPoolType == IdentityPoolType.App).ToList();


            if (connectorIdentities.Any(c => c.RelationshipTemplates.Count == 0))
            {
                var materializedConnectorIdentities = connectorIdentities
                    .Where(c => c.RelationshipTemplates.Count == 0)
                    .Select(c => $"{c.IdentityAddress}/{c.ConfigurationIdentityAddress}/{c.PoolAlias} {IDENTITY_LOG_SUFFIX}")
                    .ToList();

                throw new InvalidOperationException("One or more relationship target connector identities do not have a usable relationship template." +
                                                    Environment.NewLine +
                                                    "Connector identities:" +
                                                    Environment.NewLine +
                                                    $"{string.Join($",{Environment.NewLine}", materializedConnectorIdentities)}");
            }


            foreach (var appIdentity in appIdentities)
            {
                var connectorRecipientIds = request.RelationshipAndMessages
                    .Where(relationship =>
                        appIdentity.PoolAlias == relationship.SenderPoolAlias &&
                        appIdentity.ConfigurationIdentityAddress == relationship.SenderIdentityAddress)
                    .Select(relationship => new RelationshipIdBag(relationship.RecipientIdentityAddress, relationship.RecipientPoolAlias))
                    .ToList();

                var connectorIdentityToEstablishRelationshipWith = connectorIdentities
                    .Where(connectorIdentity => connectorRecipientIds.Any(relationship =>
                        connectorIdentity.PoolAlias == relationship.PoolAlias &&
                        connectorIdentity.ConfigurationIdentityAddress == relationship.IdentityAddress))
                    .ToList();

                var connectorIdentityToEstablishRelationshipWithIds = connectorIdentityToEstablishRelationshipWith
                    .Select(c => new RelationshipIdBag(c.ConfigurationIdentityAddress, c.PoolAlias))
                    .ToList();

                if (!connectorRecipientIds.SequenceEqual(connectorIdentityToEstablishRelationshipWithIds))
                {
                    throw new InvalidOperationException(BuildRelationshipErrorDetails("Mismatch between configured relationships and connector identities.",
                        appIdentity,
                        connectorRecipientIds,
                        connectorIdentityToEstablishRelationshipWithIds));
                }

                foreach (var connectorIdentity in connectorIdentityToEstablishRelationshipWith)
                {
                    var appIdentitySdkClient = Client.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, appIdentity.UserCredentials);
                    var connectorIdentitySdkClient = Client.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, connectorIdentity.UserCredentials);

                    var nextRelationshipTemplate = connectorIdentity.RelationshipTemplates.FirstOrDefault(t => t.Used == false);

                    if (nextRelationshipTemplate == default)
                    {
                        throw new InvalidOperationException("Connector Identity has no further RelationshipTemplates." +
                                                            Environment.NewLine +
                                                            $"connector-identity: {connectorIdentity.IdentityAddress}/{connectorIdentity.ConfigurationIdentityAddress}/{connectorIdentity.IdentityPoolType} {IDENTITY_LOG_SUFFIX}");
                    }

                    nextRelationshipTemplate.Used = true;

                    var createRelationshipResponse = await appIdentitySdkClient.Relationships.CreateRelationship(
                        new CreateRelationshipRequest
                        {
                            RelationshipTemplateId = nextRelationshipTemplate.Template.Id,
                            Content = []
                        });

                    if (createRelationshipResponse.IsError)
                    {
                        throw new InvalidOperationException(BuildErrorDetails(
                            "Failed to create relationship.",
                            appIdentity,
                            connectorIdentity,
                            createRelationshipResponse));
                    }

                    var acceptRelationshipResponse = await connectorIdentitySdkClient.Relationships.AcceptRelationship(
                        createRelationshipResponse.Result!.Id,
                        new AcceptRelationshipRequest());

                    if (acceptRelationshipResponse.IsError)
                    {
                        throw new InvalidOperationException(BuildErrorDetails(
                            "Failed to accept relationship.",
                            appIdentity,
                            connectorIdentity,
                            acceptRelationshipResponse));
                    }

                    if (acceptRelationshipResponse.Result is null) continue;

                    appIdentity.EstablishedRelationshipsById.Add(acceptRelationshipResponse.Result.Id, connectorIdentity);
                }
            }

            return Unit.Value;
        }
    }
}
