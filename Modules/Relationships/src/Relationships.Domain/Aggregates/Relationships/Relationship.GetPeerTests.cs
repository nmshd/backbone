using Backbone.UnitTestTools.BaseClasses;
using FluentAssertions;
using Xunit;
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
        peer.Should().Be(relationship.To);
    }

    [Fact]
    public void Returns_to_if_from_is_passed()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        var peer = relationship.GetPeerOf(relationship.To);

        // Assert
        peer.Should().Be(relationship.From);
    }
}
