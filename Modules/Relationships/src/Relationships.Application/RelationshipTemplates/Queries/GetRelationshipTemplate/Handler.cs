using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.GetRelationshipTemplate;

public class Handler : IRequestHandler<GetRelationshipTemplateQuery, RelationshipTemplateDTO>
{
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IRelationshipTemplatesRepository relationshipTemplatesRepository)
    {
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
        _userContext = userContext;
    }

    public async Task<RelationshipTemplateDTO> Handle(GetRelationshipTemplateQuery request, CancellationToken cancellationToken)
    {
        var template = await GetRelationshipTemplate(request.Id, request.Password, cancellationToken);

        template.AllocateFor(_userContext.GetAddress(), _userContext.GetDeviceId());
        await _relationshipTemplatesRepository.Update(template);

        return new RelationshipTemplateDTO(template);
    }

    private async Task<RelationshipTemplate> GetRelationshipTemplate(string relationshipTemplateId, byte[]? password, CancellationToken cancellationToken)
    {
        var relationshipTemplate = await _relationshipTemplatesRepository.Get(RelationshipTemplateId.Parse(relationshipTemplateId), _userContext.GetAddress(), cancellationToken, true) ??
                                   throw new NotFoundException(nameof(RelationshipTemplate));

        if (!relationshipTemplate.CanBeCollectedUsingPassword(_userContext.GetAddress(), password))
            throw new NotFoundException(nameof(RelationshipTemplate));

        return relationshipTemplate;
    }
}
