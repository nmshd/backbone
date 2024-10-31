using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Enums;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;
using MediatR;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Mediator;

public record CreateRelationships
{
    public record Command(List<RelationshipAndMessages> RelationshipAndMessages, List<DomainIdentity> Identities, string BaseUrlAddress, ClientCredentials ClientCredentials)
        : IRequest<List<DomainIdentity>>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 
    public record CommandHandler : IRequestHandler<Command, List<DomainIdentity>>
    {
        public async Task<List<DomainIdentity>> Handle(Command request, CancellationToken cancellationToken)
        {
            var connectorIdentities = request.Identities.Where(i => i.IdentityPoolType == IdentityPoolType.Connector).ToList();
            var appIdentities = request.Identities.Where(i => i.IdentityPoolType == IdentityPoolType.App).ToList();


            if (connectorIdentities.Any(c => c.RelationshipTemplates.Count < 1))
                throw new Exception("One or more relationship target connector identities do not have a usable relationship template.");


            foreach (var appIdentity in appIdentities)
            {
                var connectorRecipientIds = request.RelationshipAndMessages
                    .Where(r => r.RecipientIdentityPoolType == IdentityPoolType.Connector)
                    .Where(r => r.SenderIdentityAddress == appIdentity.IdentityConfigurationAddress)
                    .Select(r => r.RecipientIdentityAddress)
                    .ToList();

                var connectorIdentityToEstablishRelationshipWith = connectorIdentities
                    .Where(c => connectorRecipientIds.Contains(c.IdentityConfigurationAddress))
                    .ToList();

                foreach (var connectorIdentity in connectorIdentityToEstablishRelationshipWith)
                {
                    var appIdentitySdkClient = Client.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, appIdentity.UserCredentials);
                    var connectorIdentitySdkClient = Client.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, connectorIdentity.UserCredentials);

                    var nextRelationshipTemplate = connectorIdentity.RelationshipTemplates.First();
                    var index = connectorIdentity.RelationshipTemplates.IndexOf(nextRelationshipTemplate);
                    connectorIdentity.RelationshipTemplates.RemoveAt(index);

                    var createRelationshipResponse = await appIdentitySdkClient.Relationships.CreateRelationship(
                        new CreateRelationshipRequest
                        {
                            RelationshipTemplateId = nextRelationshipTemplate.Id,
                            Content = []
                        });

                    var acceptRelationshipResponse = await connectorIdentitySdkClient.Relationships.AcceptRelationship(
                        createRelationshipResponse.Result!.Id,
                        new AcceptRelationshipRequest());


                    if (acceptRelationshipResponse.Result is null) continue;

                    appIdentity.EstablishedRelationshipsById.Add(acceptRelationshipResponse.Result.Id, connectorIdentity);
                }
            }

            return request.Identities;
        }
    }
}
