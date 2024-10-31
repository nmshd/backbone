using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;
using Backbone.Tooling.Extensions;
using MediatR;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Mediator;

public record CreateRelationshipTemplates
{
    public record Command(
        List<DomainIdentity> Identities,
        string BaseUrlAddress,
        ClientCredentials ClientCredentials) : IRequest<List<DomainIdentity>>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 
    public record CommandHandler : IRequestHandler<Command, List<DomainIdentity>>
    {
        public async Task<List<DomainIdentity>> Handle(Command request, CancellationToken cancellationToken)
        {
            var identitiesWithRelationshipTemplates = request.Identities.Where(i => i.NumberOfRelationshipTemplates > 0);

            foreach (var identity in identitiesWithRelationshipTemplates)
            {
                var sdkClient = Client.CreateForExistingIdentity(request.BaseUrlAddress, request.ClientCredentials, identity.UserCredentials);

                for (var i = 0; i < identity.NumberOfRelationshipTemplates; i++)
                {
                    var relationshipTemplateResponse = await sdkClient.RelationshipTemplates.CreateTemplate(
                        new CreateRelationshipTemplateRequest
                        {
                            Content = [],
                            ExpiresAt = DateTime.Now.EndOfYear(),
                            MaxNumberOfAllocations = 1000
                        });

                    if (relationshipTemplateResponse.Result is null) continue;

                    identity.RelationshipTemplates.Add(relationshipTemplateResponse.Result);
                }
            }

            return request.Identities;
        }
    }
}
