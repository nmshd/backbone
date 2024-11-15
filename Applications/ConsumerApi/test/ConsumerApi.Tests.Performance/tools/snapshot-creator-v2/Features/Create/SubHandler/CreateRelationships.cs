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


            if (connectorIdentities.Any(c => c.RelationshipTemplates.Count < 1))
                throw new Exception("One or more relationship target connector identities do not have a usable relationship template.");


            foreach (var appIdentity in appIdentities)
            {
                var connectorRecipientIds = request.RelationshipAndMessages
                    .Where(relationship =>
                        appIdentity.PoolAlias == relationship.SenderPoolAlias &&
                        appIdentity.ConfigurationIdentityAddress == relationship.SenderIdentityAddress)
                    .Select(relationship =>
                    (
                        relationship.RecipientIdentityAddress,
                        relationship.RecipientPoolAlias
                    ))
                    .ToList();

                var connectorIdentityToEstablishRelationshipWith = connectorIdentities
                    .Where(connectorIdentity => connectorRecipientIds.Any(relationship =>
                        connectorIdentity.PoolAlias == relationship.RecipientPoolAlias &&
                        connectorIdentity.ConfigurationIdentityAddress == relationship.RecipientIdentityAddress))
                    .ToList();

                List<DomainIdentity> recipientIdentities;

                // note: it can happen that an app identity has more than one relationship to the same connector identity
                if (connectorRecipientIds.Count > connectorIdentityToEstablishRelationshipWith.Count)
                {
                    recipientIdentities = [];

                    foreach (var (recipientIdentityAddress, recipientPoolAlias) in connectorRecipientIds)
                    {
                        var recipientIdentity =
                            connectorIdentityToEstablishRelationshipWith
                                .FirstOrDefault(c =>
                                    c.PoolAlias == recipientPoolAlias &&
                                    c.ConfigurationIdentityAddress == recipientIdentityAddress) ??
                            throw new InvalidOperationException($"Recipient identity with Address '{recipientIdentityAddress}' in Pool {recipientPoolAlias}" +
                                                                $" not found in {nameof(connectorIdentityToEstablishRelationshipWith)}");

                        recipientIdentities.Add(recipientIdentity);
                    }
                }
                else
                {
                    recipientIdentities = connectorIdentityToEstablishRelationshipWith;
                }

                foreach (var recipientIdentity in recipientIdentities)
                {
                    var appIdentitySdkClient = Client.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, appIdentity.UserCredentials);
                    var connectorIdentitySdkClient = Client.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, recipientIdentity.UserCredentials);

                    var nextRelationshipTemplate = recipientIdentity.RelationshipTemplates.FirstOrDefault(t => t.Used == false);

                    if (nextRelationshipTemplate == default)
                    {
                        throw new InvalidOperationException(
                            $"Connector Identity {recipientIdentity.IdentityAddress}/{recipientIdentity.ConfigurationIdentityAddress}/{recipientIdentity.IdentityPoolType}" +
                            $" [IdentityAddress/ConfigurationIdentityAddress/PoolAlias] has no further RelationshipTemplates.");
                    }

                    nextRelationshipTemplate.Used = true;

                    var createRelationshipResponse = await appIdentitySdkClient.Relationships.CreateRelationship(
                        new CreateRelationshipRequest
                        {
                            RelationshipTemplateId = nextRelationshipTemplate.Template.Id,
                            Content = []
                        });

                    var acceptRelationshipResponse = await connectorIdentitySdkClient.Relationships.AcceptRelationship(
                        createRelationshipResponse.Result!.Id,
                        new AcceptRelationshipRequest());


                    if (acceptRelationshipResponse.Result is null) continue;

                    appIdentity.EstablishedRelationshipsById.Add(acceptRelationshipResponse.Result.Id, recipientIdentity);
                }
            }

            return Unit.Value;
        }
    }
}
