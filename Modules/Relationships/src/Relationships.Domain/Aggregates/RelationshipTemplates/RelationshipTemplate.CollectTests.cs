using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Relationships.Domain.TestHelpers;

namespace Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

public class RelationshipTemplateCollectTests : AbstractTestsBase
{
    [Fact]
    public void Raises_RelationshipTemplateAllocationsExhaustedDomainEvent_when_allocations_are_maxed_out()
    {
        // Arrange
        var template = TestData.CreateRelationshipTemplate(CreateRandomIdentityAddress(), null, null, 1);
        template.ClearDomainEvents();

        // Act
        template.AllocateFor(CreateRandomIdentityAddress(), CreateRandomDeviceId());

        // Assert
        template.Should().HaveASingleDomainEvent<RelationshipTemplateAllocationsExhaustedDomainEvent>();
    }
}
