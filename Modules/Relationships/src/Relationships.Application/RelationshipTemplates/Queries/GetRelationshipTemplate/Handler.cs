using AutoMapper;
using Backbone.Modules.Relationships.Application.Extensions;
using Backbone.Modules.Relationships.Application.Infrastructure;
using Backbone.Modules.Relationships.Application.Relationships;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.GetRelationshipTemplate;

public class Handler : RequestHandlerBase<GetRelationshipTemplateQuery, RelationshipTemplateDTO>
{
    private readonly IBlobStorage _blobStorage;

    public Handler(IRelationshipsDbContext dbContext, IUserContext userContext, IMapper mapper, IBlobStorage blobStorage) : base(dbContext, userContext, mapper)
    {
        _blobStorage = blobStorage;
    }

    public override async Task<RelationshipTemplateDTO> Handle(GetRelationshipTemplateQuery request, CancellationToken cancellationToken)
    {
        var template = await _dbContext
            .Set<RelationshipTemplate>()
            .Include(r => r.Allocations)
            .NotExpiredFor(_activeIdentity)
            .NotDeleted()
            .FirstWithId(request.Id, cancellationToken);

        template.AllocateFor(_activeIdentity, _activeDevice);

        _dbContext.Set<RelationshipTemplate>().Update(template);
        await _dbContext.SaveChangesAsync(cancellationToken);

        template.LoadContent(await _blobStorage.FindAsync(template.Id));

        return _mapper.Map<RelationshipTemplateDTO>(template);
    }
}
