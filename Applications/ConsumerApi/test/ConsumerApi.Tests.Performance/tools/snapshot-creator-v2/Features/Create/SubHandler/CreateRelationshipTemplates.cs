using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Backbone.Tooling.Extensions;
using MediatR;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public record CreateRelationshipTemplates
{
    public record Command(
        List<DomainIdentity> Identities,
        string BaseUrlAddress,
        ClientCredentials ClientCredentials) : IRequest<Unit>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 
    public record CommandHandler : IRequestHandler<Command, Unit>
    {
        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
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

                    if (relationshipTemplateResponse.IsError)
                    {
                        throw new InvalidOperationException(BuildErrorDetails(
                            "Failed to create relationship template.",
                            identity,
                            relationshipTemplateResponse));
                    }

                    if (relationshipTemplateResponse.Result is null) continue;

                    identity.RelationshipTemplates.Add(new RelationshipTemplateBag(relationshipTemplateResponse.Result, false));
                }
            }

            return Unit.Value;
        }
    }
}
