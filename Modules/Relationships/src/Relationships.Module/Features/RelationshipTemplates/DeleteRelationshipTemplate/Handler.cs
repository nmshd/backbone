using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Abstractions;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.DeleteRelationshipTemplate;

public class Handler : IRequestHandler<Command>
{
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;
    private readonly IUserContext _userContext;

    public Handler(IRelationshipTemplatesRepository relationshipTemplatesRepository, IUserContext userContext)
    {
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
        _userContext = userContext;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        var relationshipTemplate = await _relationshipTemplatesRepository.GetWithoutContent(RelationshipTemplateId.Parse(request.Id), _userContext.GetAddress(), cancellationToken) ??
                                   throw new NotFoundException(nameof(RelationshipTemplate));

        relationshipTemplate.EnsureCanBeDeletedBy(_userContext.GetAddress());

        await _relationshipTemplatesRepository.Delete(relationshipTemplate, cancellationToken);
    }
}
