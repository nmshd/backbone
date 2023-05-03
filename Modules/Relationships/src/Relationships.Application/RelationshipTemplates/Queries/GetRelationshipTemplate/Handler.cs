using AutoMapper;
using Backbone.Modules.Relationships.Application.Infrastructure;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.Relationships;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.GetRelationshipTemplate;

public class Handler : RequestHandlerBase<GetRelationshipTemplateQuery, RelationshipTemplateDTO>
{
    private readonly IRelationshipsRepository _relationshipsRepository;

    // TODO: Remove dbContext
    public Handler(IRelationshipsDbContext dbContext, IUserContext userContext, IMapper mapper, IRelationshipsRepository relationshipsRepository) : base(dbContext, userContext, mapper)
    {
        _relationshipsRepository = relationshipsRepository;
    }

    public override async Task<RelationshipTemplateDTO> Handle(GetRelationshipTemplateQuery request, CancellationToken cancellationToken)
    {
        var template = await _relationshipsRepository.FindRelationshipTemplate(request.Id, _activeIdentity, cancellationToken, track: true);

        template.AllocateFor(_activeIdentity, _activeDevice);

        await _relationshipsRepository.UpdateRelationshipTemplate(template);

        return _mapper.Map<RelationshipTemplateDTO>(template);
    }
}
