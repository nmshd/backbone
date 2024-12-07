using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.UnitTestTools.Extensions;

namespace Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

public class RelationshipTemplateAnonymizeForIdentityTests : AbstractTestsBase
{
    private const string DID_DOMAIN_NAME = "localhost";

    [Fact]
    public void Personalized_template_can_be_anonymized()
    {
        // Arrange
        var creatorIdentityAddress = CreateRandomIdentityAddress();
        var forIdentityAddress = CreateRandomIdentityAddress();
        var deviceId = CreateRandomDeviceId();
        byte[] content = [1, 1, 1, 1, 1, 1, 1, 1];
        var relationshipTemplate = new RelationshipTemplate(creatorIdentityAddress, deviceId, null, null, content, forIdentityAddress);

        // Act
        relationshipTemplate.AnonymizeForIdentity(DID_DOMAIN_NAME);

        // Assert
        relationshipTemplate.ForIdentity.Should().Be(IdentityAddress.GetAnonymized(DID_DOMAIN_NAME));
    }

    [Fact]
    public void Non_personalized_template_can_not_be_anonymized()
    {
        // Arrange
        var creatorIdentityAddress = CreateRandomIdentityAddress();
        var deviceId = CreateRandomDeviceId();
        byte[] content = [1, 1, 1, 1, 1, 1, 1, 1];
        var relationshipTemplate = new RelationshipTemplate(creatorIdentityAddress, deviceId, null, _dateTimeNow, content);

        // Act
        var acting = () => relationshipTemplate.AnonymizeForIdentity(DID_DOMAIN_NAME);

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationship.relationshipTemplateNotPersonalized");
    }
}
