using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.ExecuteHousekeeping;

public class Handler : IRequestHandler<ExecuteHousekeepingCommand>
{
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;
    private readonly ILogger<Handler> _logger;

    public Handler(IRelationshipTemplatesRepository relationshipTemplatesRepository, ILogger<Handler> logger)
    {
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
        _logger = logger;
    }

    public async Task Handle(ExecuteHousekeepingCommand request, CancellationToken cancellationToken)
    {
        await DeleteRelationshipTemplates(cancellationToken);
    }

    private async Task DeleteRelationshipTemplates(CancellationToken cancellationToken)
    {
        var numberOfDeletedTemplates = await _relationshipTemplatesRepository.Delete(RelationshipTemplate.CanBeCleanedUp, cancellationToken);

        _logger.LogInformation("Deleted {numberOfDeletedItems} relationship templates", numberOfDeletedTemplates);
    }
}
