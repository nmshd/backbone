using Backbone.BuildingBlocks.Application.Housekeeping;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.ExecuteHousekeeping;

public class Handler : IRequestHandler<ExecuteHousekeepingCommand>
{
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;
    private readonly ILogger<Handler> _logger;
    private readonly IRelationshipsRepository _relationshipsRepository;

    public Handler(IRelationshipTemplatesRepository relationshipTemplatesRepository, IRelationshipsRepository relationshipsRepository, ILogger<Handler> logger)
    {
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
        _relationshipsRepository = relationshipsRepository;
        _logger = logger;
    }

    public async Task Handle(ExecuteHousekeepingCommand request, CancellationToken cancellationToken)
    {
        await DeleteRelationshipTemplates(cancellationToken);
        await DeleteRelationships(cancellationToken);
    }

    private async Task DeleteRelationshipTemplates(CancellationToken cancellationToken)
    {
        var numberOfDeletedItems = await _relationshipTemplatesRepository.Delete(RelationshipTemplate.CanBeCleanedUp, cancellationToken);

        _logger.DataDeleted(numberOfDeletedItems, "relationship templates");
    }

    private async Task DeleteRelationships(CancellationToken cancellationToken)
    {
        var numberOfDeletedItems = await _relationshipsRepository.Delete(Relationship.CanBeCleanedUp, cancellationToken);

        _logger.DataDeleted(numberOfDeletedItems, "relationships");
    }
}
