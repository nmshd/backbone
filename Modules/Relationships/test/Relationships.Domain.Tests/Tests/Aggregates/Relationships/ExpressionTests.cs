using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Tests.TestHelpers;
using Backbone.UnitTestTools.BaseClasses;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Relationships.Domain.Tests.Tests.Aggregates.Relationships;

public class ExpressionTests : AbstractTestsBase
{
    #region CountsAsActive

    [Fact]
    public void CountsAsActive_with_status_Pending()
    {
        // Arrange
        var pendingRelationship = TestData.CreatePendingRelationship();

        // Act
        var result = pendingRelationship.EvaluateCountsAsActiveExpression();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CountsAsActive_with_status_Active()
    {
        // Arrange
        var activeRelationship = TestData.CreateActiveRelationship();

        // Act
        var result = activeRelationship.EvaluateCountsAsActiveExpression();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CountsAsActive_with_status_Rejected()
    {
        // Arrange
        var rejectedRelationship = TestData.CreateRejectedRelationship();

        // Act
        var result = rejectedRelationship.EvaluateCountsAsActiveExpression();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CountsAsActive_with_status_Revoked()
    {
        // Arrange
        var revokedRelationship = TestData.CreateRevokedRelationship();

        // Act
        var result = revokedRelationship.EvaluateCountsAsActiveExpression();

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region HasParticipant

    [Fact]
    public void HasParticipant_recognizes_from_address()
    {
        // Arrange
        var relationship = TestData.CreateActiveRelationship();

        // Act
        var result = relationship.EvaluateHasParticipantExpression(relationship.From);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasParticipant_recognizes_to_address()
    {
        // Arrange
        var relationship = TestData.CreateActiveRelationship();

        // Act
        var result = relationship.EvaluateHasParticipantExpression(relationship.To);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasParticipant_recognizes_foreign_addresses()
    {
        // Arrange
        var relationship = TestData.CreateActiveRelationship();

        // Act
        var result = relationship.EvaluateHasParticipantExpression("did:e:localhost:dids:1111111111111111111111");

        // Assert
        result.Should().BeFalse();
    }

    #endregion
}

file static class RelationshipExtensions
{
    public static bool EvaluateHasParticipantExpression(this Relationship relationship, string identity)
    {
        var expression = Relationship.HasParticipant(identity);
        return expression.Compile()(relationship);
    }

    public static bool EvaluateCountsAsActiveExpression(this Relationship relationship)
    {
        var expression = Relationship.CountsAsActive();
        return expression.Compile()(relationship);
    }
}
