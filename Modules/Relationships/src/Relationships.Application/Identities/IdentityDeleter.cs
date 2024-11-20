using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Relationships.Commands.DecomposeAndAnonymizeRelationshipsOfIdentity;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.AnonymizeRelationshipTemplateAllocationsAllocatedByIdentity;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.DeleteRelationshipTemplatesOfIdentity;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Identities;

public class IdentityDeleter : IIdentityDeleter
{
    private readonly IMediator _mediator;
    private readonly IDeletionProcessLogger _deletionProcessLogger;

    public IdentityDeleter(IMediator mediator, IDeletionProcessLogger deletionProcessLogger)
    {
        _mediator = mediator;
        _deletionProcessLogger = deletionProcessLogger;
    }

    public async Task Delete(IdentityAddress identityAddress)
    {
        await _mediator.Send(new DecomposeAndAnonymizeRelationshipsOfIdentityCommand(identityAddress));
        await _deletionProcessLogger.LogDeletion(identityAddress, "Relationships");
        await _mediator.Send(new DeleteRelationshipTemplatesOfIdentityCommand(identityAddress));
        await _deletionProcessLogger.LogDeletion(identityAddress, "RelationshipTemplates");
        await _mediator.Send(new AnonymizeRelationshipTemplateAllocationsAllocatedByIdentityCommand(identityAddress));
        await _deletionProcessLogger.LogDeletion(identityAddress, "RelationshipTemplateAllocations");
    }
}
