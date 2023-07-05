using Backbone.Modules.Relationships.Domain.Errors;
using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Relationships.Domain.Entities;

public class RelationshipCreationChange : RelationshipChange
{
    private RelationshipCreationChange() { }

    internal RelationshipCreationChange(Relationship relationship, IdentityAddress createdBy, DeviceId createdByDevice, byte[]? requestContent) : base(relationship, createdBy, createdByDevice, RelationshipChangeType.Creation, requestContent) { }

    protected override void EnsureCanBeAccepted(IdentityAddress by, byte[]? content)
    {
        if (content == null)
            throw new DomainException(DomainErrors.ContentIsRequiredForCompletingRelationships());

        base.EnsureCanBeAccepted(by, content);
    }

    protected override void EnsureCanBeRejected(IdentityAddress by, byte[]? content)
    {
        if (content == null)
            throw new DomainException(DomainErrors.ContentIsRequiredForCompletingRelationships());

        base.EnsureCanBeRejected(by, content);
    }

    protected override void EnsureCanBeRevoked(IdentityAddress by, byte[]? content)
    {
        if (content == null)
            throw new DomainException(DomainErrors.ContentIsRequiredForCompletingRelationships());

        base.EnsureCanBeRevoked(by, content);
    }
}
