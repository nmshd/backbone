using AutoMapper;
using Backbone.Modules.Relationships.Application.Extensions;
using Backbone.Modules.Relationships.Application.Infrastructure;
using Backbone.Modules.Relationships.Application.Relationships;
using Backbone.Modules.Relationships.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.Tooling;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.DeleteRelationshipTemplate;

public class Handler : RequestHandlerBase<DeleteRelationshipTemplateCommand, Unit>
{
    public Handler(IRelationshipsDbContext dbContext, IUserContext userContext, IMapper mapper) : base(dbContext, userContext, mapper) { }

    public override async Task<Unit> Handle(DeleteRelationshipTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = await _dbContext.Set<RelationshipTemplate>().FirstWithId(request.Id, cancellationToken);

        if (template.CreatedBy != _activeIdentity)
            throw new ActionForbiddenException();

        template.DeletedAt = SystemTime.UtcNow;
        _dbContext.Set<RelationshipTemplate>().Update(template);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
