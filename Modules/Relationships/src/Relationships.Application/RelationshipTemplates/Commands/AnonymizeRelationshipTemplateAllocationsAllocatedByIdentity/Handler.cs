using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using MediatR;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.AnonymizeRelationshipTemplateAllocationsAllocatedByIdentity;

public class Handler : IRequestHandler<AnonymizeRelationshipTemplateAllocationsAllocatedByIdentityCommand>
{
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;
    private readonly ApplicationConfiguration _applicationConfiguration;

    public Handler(IRelationshipTemplatesRepository relationshipTemplatesRepository, IOptions<ApplicationConfiguration> options)
    {
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
        _applicationConfiguration = options.Value;
    }

    public async Task Handle(AnonymizeRelationshipTemplateAllocationsAllocatedByIdentityCommand request, CancellationToken cancellationToken)
    {
        var allocations =
            (await _relationshipTemplatesRepository.ListRelationshipTemplateAllocations(RelationshipTemplateAllocation.WasAllocatedBy(request.IdentityAddress), cancellationToken)).ToList();

        foreach (var allocation in allocations)
            allocation.Anonymize(_applicationConfiguration.DidDomainName);

        await _relationshipTemplatesRepository.UpdateRelationshipTemplateAllocations(allocations, cancellationToken);
    }
}
