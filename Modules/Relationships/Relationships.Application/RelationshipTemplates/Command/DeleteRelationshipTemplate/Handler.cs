using AutoMapper;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.Tooling;
using MediatR;
using Relationships.Application.Extensions;
using Relationships.Application.Relationships;
using Relationships.Domain.Entities;

namespace Relationships.Application.RelationshipTemplates.Command.DeleteRelationshipTemplate;

public class Handler : RequestHandlerBase<DeleteRelationshipTemplateCommand, Unit>
{
    public Handler(IDbContext dbContext, IUserContext userContext, IMapper mapper) : base(dbContext, userContext, mapper) { }

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
