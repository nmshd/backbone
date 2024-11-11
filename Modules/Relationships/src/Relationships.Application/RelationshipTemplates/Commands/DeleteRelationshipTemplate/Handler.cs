using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.DeleteRelationshipTemplate;

public class Handler : IRequestHandler<DeleteRelationshipTemplateCommand>
{
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;
    private readonly IUserContext _userContext;

    public Handler(IRelationshipTemplatesRepository relationshipTemplatesRepository, IUserContext userContext)
    {
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
        _userContext = userContext;
    }

    public async Task Handle(DeleteRelationshipTemplateCommand request, CancellationToken cancellationToken)
    {
        var relationshipTemplate = await _relationshipTemplatesRepository.Find(RelationshipTemplateId.Parse(request.Id), _userContext.GetAddress(), cancellationToken) ??
                                   throw new NotFoundException(nameof(RelationshipTemplate));

        relationshipTemplate.EnsureCanBeDeletedBy(_userContext.GetAddress());

        await _relationshipTemplatesRepository.Delete(relationshipTemplate, cancellationToken);
    }
}
