using AutoMapper;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Microsoft.EntityFrameworkCore;
using Relationships.Application.Extensions;
using Relationships.Application.Infrastructure;
using Relationships.Application.Relationships.DTOs;
using Relationships.Domain.Entities;

namespace Relationships.Application.Relationships.Queries.GetRelationship;

public class Handler : RequestHandlerBase<GetRelationshipQuery, RelationshipDTO>
{
    private readonly IContentStore _contentStore;

    public Handler(IDbContext dbContext, IUserContext userContext, IMapper mapper, IContentStore contentStore) : base(dbContext, userContext, mapper)
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
