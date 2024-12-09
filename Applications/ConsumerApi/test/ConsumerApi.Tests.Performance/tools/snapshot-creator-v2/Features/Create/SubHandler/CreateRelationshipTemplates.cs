using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using MediatR;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public abstract record CreateRelationshipTemplates
{
    public record Command(
        List<DomainIdentity> Identities,
        string BaseUrlAddress,
        ClientCredentials ClientCredentials) : IRequest<Unit>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 

    public class CommandHandler(IRelationshipTemplateFactory relationshipTemplateFactory) : IRequestHandler<Command, Unit>
    {
        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var identitiesWithRelationshipTemplates = request.Identities
                .Where(i => i.NumberOfRelationshipTemplates > 0)
                .ToArray();

            relationshipTemplateFactory.TotalConfiguredRelationshipTemplates = identitiesWithRelationshipTemplates.Sum(i => i.NumberOfRelationshipTemplates);

            var tasks = identitiesWithRelationshipTemplates
                .Select(identityWithRelationshipTemplate => relationshipTemplateFactory.Create(request, identityWithRelationshipTemplate))
                .ToArray();

            await Task.WhenAll(tasks);

            return Unit.Value;
        }
    }
}
