using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using NeoSmart.Utils;

namespace Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

public class RelationshipTemplateDeleteTests : AbstractTestsBase
{
    [Fact]
    public void RelationshipTemplate_can_be_deleted_by_its_owner()
    {
        var identity = CreateRandomIdentityAddress();
        var relationshipTemplate = CreateRelationshipTemplate(identity);

        var acting = () => relationshipTemplate.EnsureCanBeDeletedBy(identity);

        acting.ShouldNotThrow();
    }

    [Fact]
    public void RelationshipTemplate_can_not_be_deleted_by_others()
    {
        var creatorIdentity = CreateRandomIdentityAddress();
        var otherIdentity = CreateRandomIdentityAddress();
        var relationshipTemplate = CreateRelationshipTemplate(creatorIdentity);

        var acting = () => relationshipTemplate.EnsureCanBeDeletedBy(otherIdentity);

        acting.ShouldThrow<DomainActionForbiddenException>();
    }

    private RelationshipTemplate CreateRelationshipTemplate(IdentityAddress identity)
    {
        var deviceId = CreateRandomDeviceId();
        var content = UrlBase64.Decode("AAAA");

        return new RelationshipTemplate(identity, deviceId, null, null, content);
    }
}
