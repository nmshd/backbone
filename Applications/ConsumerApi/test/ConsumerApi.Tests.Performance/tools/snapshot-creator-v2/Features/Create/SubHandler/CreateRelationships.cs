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
                    .Where(r =>
                        r.RecipientIdentityPoolType == IdentityPoolType.Connector &&
                        r.SenderIdentityAddress == appIdentity.ConfigurationIdentityAddress)
                    .Select(r =>
                    (
                        r.RecipientIdentityAddress,
                        r.RecipientPoolAlias
                    ))
                    .ToList();

                var connectorIdentityToEstablishRelationshipWith = connectorIdentities
                    .Where(c => connectorRecipientIds.Any(cr =>
                        cr.RecipientPoolAlias == c.PoolAlias &&
                        cr.RecipientIdentityAddress == c.ConfigurationIdentityAddress))
                    .ToList();

                foreach (var connectorIdentity in connectorIdentityToEstablishRelationshipWith)
                {
                    var appIdentitySdkClient = Client.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, appIdentity.UserCredentials);
                    var connectorIdentitySdkClient = Client.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, connectorIdentity.UserCredentials);
                    var nextRelationshipTemplate = connectorIdentity.RelationshipTemplates.FirstOrDefault(t => t.Used == false);

                    if (nextRelationshipTemplate == default)
                    {
                        throw new InvalidOperationException(
                            $"Connector Identity {connectorIdentity.IdentityAddress}/{connectorIdentity.ConfigurationIdentityAddress}/{connectorIdentity.IdentityPoolType}" +
                            $" [IdentityAddress/ConfigAddress/Pool] has no further RelationshipTemplates.");
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

                    appIdentity.EstablishedRelationshipsById.Add(acceptRelationshipResponse.Result.Id, connectorIdentity);
                }
            }

            return Unit.Value;
        }
    }
}
