using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

public class RelationshipTemplateCreateTests : AbstractTestsBase
{
    [Theory]
    [InlineData(null)]
    [InlineData("did:e:prod.enmeshed.eu:dids:70cf4f3e6edf6bca33d35f")]
    public void Raises_RelationshipTemplateCreatedDomainEvent(string? forAddress)
    {
        // Arrange
        var address = CreateRandomIdentityAddress();
        var forIdentity = forAddress == null ? null : IdentityAddress.ParseUnsafe(forAddress);
        var deviceId = CreateRandomDeviceId();
        var expiresAt = DateTime.UtcNow;
        byte[] content = [1, 1, 1, 1, 1, 1, 1, 1];

        // Act
        var template = new RelationshipTemplate(address, deviceId, null, expiresAt, content, forIdentity);

        // Assert
        var domainEvent = template.ShouldHaveASingleDomainEvent<RelationshipTemplateCreatedDomainEvent>();
        domainEvent.TemplateId.ShouldBe(template.Id);
        domainEvent.CreatedBy.ShouldBe(address);
    }
}
