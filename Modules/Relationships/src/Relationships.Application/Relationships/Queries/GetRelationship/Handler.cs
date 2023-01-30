using AutoMapper;
using Backbone.Modules.Relationships.Application.Extensions;
using Backbone.Modules.Relationships.Application.Infrastructure;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.GetRelationship;

public class Handler : RequestHandlerBase<GetRelationshipQuery, RelationshipDTO>
{
    private readonly IContentStore _contentStore;

    public Handler(IRelationshipsDbContext dbContext, IUserContext userContext, IMapper mapper, IContentStore contentStore) : base(dbContext, userContext, mapper)
    {
        _contentStore = contentStore;
    }

    public override async Task<RelationshipDTO> Handle(GetRelationshipQuery request, CancellationToken cancellationToken)
    {
        var relationship = await _dbContext
            .Set<Relationship>()
            .IncludeAll()
            .AsNoTracking()
            .WithParticipant(_activeIdentity)
            .FirstWithId(request.Id, cancellationToken);

        await _contentStore.FillContentOfChanges(relationship.Changes);

        return _mapper.Map<RelationshipDTO>(relationship);
    }
}
