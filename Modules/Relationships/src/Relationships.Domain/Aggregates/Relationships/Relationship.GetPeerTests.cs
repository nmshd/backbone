using static Backbone.Modules.Relationships.Domain.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class RelationshipGetPeerTests : AbstractTestsBase
{
    [Fact]
    public void Returns_from_if_to_is_passed()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        var peer = relationship.GetPeerOf(relationship.From);

        // Assert
        peer.ShouldBe(relationship.To);
    }

    [Fact]
    public void Returns_to_if_from_is_passed()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        var peer = relationship.GetPeerOf(relationship.To);

        // Assert
        peer.ShouldBe(relationship.From);
    }
}
