using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using NeoSmart.Utils;

namespace Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

public class RelationshipTemplateDeleteTests : AbstractTestsBase
{
    [Fact]
    public void RelationshipTemplateCanBeDeletedByItsOwner()
    {
        var identity = CreateRandomIdentityAddress();
        var relationshipTemplate = CreateRelationshipTemplate(identity);

        var func = () => relationshipTemplate.EnsureCanBeDeletedBy(identity);

        func.Should().NotThrow();
    }

    [Fact]
    public void RelationshipTemplateCanNotBeDeletedByOthers()
    {
        var creatorIdentity = CreateRandomIdentityAddress();
        var otherIdentity = CreateRandomIdentityAddress();
        var relationshipTemplate = CreateRelationshipTemplate(creatorIdentity);

        var func = () => relationshipTemplate.EnsureCanBeDeletedBy(otherIdentity);

        func.Should().Throw<DomainActionForbiddenException>();
    }

    private RelationshipTemplate CreateRelationshipTemplate(IdentityAddress identity)
    {
        var deviceId = CreateRandomDeviceId();
        var content = UrlBase64.Decode("AAAA");

        return new RelationshipTemplate(identity, deviceId, null, null, content, null, null);
    }
}
