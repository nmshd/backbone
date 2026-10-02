using Backbone.BuildingBlocks.Application.Housekeeping;
using Backbone.Modules.Relationships.Abstractions;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.ExecuteHousekeeping;

public class Handler : IRequestHandler<Command>
{
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;
    private readonly IRelationshipsRepository _relationshipsRepository;

    public Handler(IRelationshipTemplatesRepository relationshipTemplatesRepository, IRelationshipsRepository relationshipsRepository)
    {
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
        _relationshipsRepository = relationshipsRepository;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        await DeleteRelationshipTemplates(cancellationToken);
        await DeleteRelationships(cancellationToken);
    }

    private async Task DeleteRelationshipTemplates(CancellationToken cancellationToken)
    {
        await HousekeepingTelemetry.TrackItemDeletion("relationship templates", ct => _relationshipTemplatesRepository.Delete(RelationshipTemplate.CanBeCleanedUp, ct), cancellationToken);
    }

    private async Task DeleteRelationships(CancellationToken cancellationToken)
    {
        await HousekeepingTelemetry.TrackItemDeletion("relationships", ct => _relationshipsRepository.Delete(Relationship.CanBeCleanedUp, ct), cancellationToken);
    }
}
