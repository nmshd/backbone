using Backbone.BuildingBlocks.Application.Housekeeping;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.ExecuteHousekeeping;

public class Handler : IRequestHandler<ExecuteHousekeepingCommand>
{
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;
    private readonly HousekeepingTelemetry _telemetry;
    private readonly IRelationshipsRepository _relationshipsRepository;

    public Handler(IRelationshipTemplatesRepository relationshipTemplatesRepository, IRelationshipsRepository relationshipsRepository, HousekeepingTelemetry telemetry)
    {
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
        _relationshipsRepository = relationshipsRepository;
        _telemetry = telemetry;
    }

    public async Task Handle(ExecuteHousekeepingCommand request, CancellationToken cancellationToken)
    {
        await DeleteRelationshipTemplates(cancellationToken);
        await DeleteRelationships(cancellationToken);
    }

    private async Task DeleteRelationshipTemplates(CancellationToken cancellationToken)
    {
        await _telemetry.TrackDeletion("relationship templates", ct => _relationshipTemplatesRepository.Delete(RelationshipTemplate.CanBeCleanedUp, ct), cancellationToken);
    }

    private async Task DeleteRelationships(CancellationToken cancellationToken)
    {
        await _telemetry.TrackDeletion("relationships", ct => _relationshipsRepository.Delete(Relationship.CanBeCleanedUp, ct), cancellationToken);
    }
}
