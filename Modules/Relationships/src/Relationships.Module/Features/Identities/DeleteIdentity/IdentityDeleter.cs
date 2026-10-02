using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;
using AnonymizeRelationshipTemplateAllocationsAllocatedByIdentity = Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.AnonymizeRelationshipTemplateAllocationsAllocatedByIdentity;
using AnonymizeRelationshipTemplatesForIdentity = Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.AnonymizeRelationshipTemplatesForIdentity;
using DecomposeAndAnonymizeRelationshipsOfIdentity = Backbone.Modules.Relationships.Module.Features.Relationships.DecomposeAndAnonymizeRelationshipsOfIdentity;
using DeleteRelationshipTemplatesOfIdentity = Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.DeleteRelationshipTemplatesOfIdentity;

namespace Backbone.Modules.Relationships.Module.Features.Identities.DeleteIdentity;

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
        await _mediator.Send(new DecomposeAndAnonymizeRelationshipsOfIdentity.Command { IdentityAddress = identityAddress });
        await _deletionProcessLogger.LogDeletion(identityAddress, "Relationships");
        await _mediator.Send(new DeleteRelationshipTemplatesOfIdentity.Command { IdentityAddress = identityAddress });
        await _mediator.Send(new AnonymizeRelationshipTemplatesForIdentity.Command { IdentityAddress = identityAddress });
        await _deletionProcessLogger.LogDeletion(identityAddress, "RelationshipTemplates");
        await _mediator.Send(new AnonymizeRelationshipTemplateAllocationsAllocatedByIdentity.Command { IdentityAddress = identityAddress });
        await _deletionProcessLogger.LogDeletion(identityAddress, "RelationshipTemplateAllocations");
    }
}
