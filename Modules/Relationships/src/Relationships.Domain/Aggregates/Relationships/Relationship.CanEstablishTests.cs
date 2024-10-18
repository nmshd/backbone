using Backbone.Modules.Relationships.Domain.TestHelpers;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class RelationshipCanEstablishTests : AbstractTestsBase
{
    [Fact]
    public void Returns_null_when_no_relationships_exists()
    {
        // Act
        var error = Relationship.CanEstablish([]);

        // Assert
        error.Should().BeNull();
    }

    [Fact]
    public void Returns_null_when_a_relationship_ready_for_deletion_exists()
    {
        // Act
        var error = Relationship.CanEstablish([TestData.CreateRelationshipInStatus(RelationshipStatus.ReadyForDeletion)]);

        // Assert
        error.Should().BeNull();
    }

    [Fact]
    public void Returns_null_when_a_rejected_relationship_exists()
    {
        // Act
        var error = Relationship.CanEstablish([TestData.CreateRelationshipInStatus(RelationshipStatus.Rejected)]);

        // Assert
        error.Should().BeNull();
    }

    [Fact]
    public void Returns_null_when_a_revoked_relationship_exists()
    {
        // Act
        var error = Relationship.CanEstablish([TestData.CreateRelationshipInStatus(RelationshipStatus.Revoked)]);

        // Assert
        error.Should().BeNull();
    }

    [Theory]
    [InlineData(RelationshipStatus.Pending)]
    [InlineData(RelationshipStatus.Active)]
    [InlineData(RelationshipStatus.Terminated)]
    [InlineData(RelationshipStatus.DeletionProposed)]
    public void Returns_an_error_when_a_relationship_exists(RelationshipStatus status)
    {
        // Act
        var error = Relationship.CanEstablish([TestData.CreateRelationshipInStatus(status)]);

        // Assert
        error.Should().NotBeNull();
        error!.Code.Should().Be("error.platform.validation.relationship.relationshipToTargetAlreadyExists");
    }
}
