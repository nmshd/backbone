using AutoMapper;
using Backbone.Modules.Relationships.Application.Extensions;
using Backbone.Modules.Relationships.Application.Infrastructure;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.GetChange;

public class Handler : IRequestHandler<GetChangeRequest, RelationshipChangeDTO>
{
    private readonly IContentStore _contentStore;
    private readonly IRelationshipsDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public Handler(IRelationshipsDbContext dbContext, IUserContext userContext, IMapper mapper, IContentStore contentStore)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _mapper = mapper;
        _contentStore = contentStore;
    }

    public async Task<RelationshipChangeDTO> Handle(GetChangeRequest request, CancellationToken cancellationToken)
    {
        var change = await _dbContext
            .Set<RelationshipChange>()
            .IncludeAll()
            .AsNoTracking()
            .WithId(request.Id)
            .WithRelationshipParticipant(_userContext.GetAddress())
            .FirstOrDefaultAsync(cancellationToken);

        await _contentStore.FillContentOfChange(change);

        return _mapper.Map<RelationshipChangeDTO>(change);
    }
}
