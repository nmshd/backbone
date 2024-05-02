using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.GetRelationshipTemplate;

public class Handler : IRequestHandler<GetRelationshipTemplateQuery, RelationshipTemplateDTO>
{
    private readonly IMapper _mapper;
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IMapper mapper, IRelationshipTemplatesRepository relationshipTemplatesRepository)
    {
        _mapper = mapper;
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
        _userContext = userContext;
    }

    public async Task<RelationshipTemplateDTO> Handle(GetRelationshipTemplateQuery request, CancellationToken cancellationToken)
    {
        var template = await _relationshipTemplatesRepository.Find(request.Id, _userContext.GetAddress(), cancellationToken, track: true) ?? throw new NotFoundException(nameof(RelationshipTemplate));

        template.AllocateFor(_userContext.GetAddress(), _userContext.GetDeviceId());

        await _relationshipTemplatesRepository.Update(template);

        return _mapper.Map<RelationshipTemplateDTO>(template);
    }
}
