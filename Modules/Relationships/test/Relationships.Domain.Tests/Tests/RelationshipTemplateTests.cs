using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using Backbone.UnitTestTools.FluentAssertions.Extensions;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Relationships.Domain.Tests.Tests;

public class RelationshipTemplateTests : AbstractTestsBase
{
    [Fact]
    public void Raises_RelationshipTemplateCreatedDomainEvent_when_creating()
    {
        // Arrange
        var address = TestDataGenerator.CreateRandomIdentityAddress();
        var deviceId = TestDataGenerator.CreateRandomDeviceId();
        var expiresAt = DateTime.UtcNow;
        byte[] content = [1, 1, 1, 1, 1, 1, 1, 1];

        // Act
        var template = new RelationshipTemplate(address, deviceId, null, expiresAt, content);

        // Assert
        var domainEvent = template.Should().HaveASingleDomainEvent<RelationshipTemplateCreatedDomainEvent>();
        domainEvent.TemplateId.Should().Be(template.Id);
        domainEvent.CreatedBy.Should().Be(address);
    }
}
