using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.TestHelpers;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class RelationshipCountAsActiveExpressionTests : AbstractTestsBase
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
        result.ShouldBeTrue();
    }

    [Fact]
    public void CountsAsActive_with_status_Active()
    {
        // Arrange
        var activeRelationship = TestData.CreateActiveRelationship();

        // Act
        var result = activeRelationship.EvaluateCountsAsActiveExpression();

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void CountsAsActive_with_status_Rejected()
    {
        // Arrange
        var rejectedRelationship = TestData.CreateRejectedRelationship();

        // Act
        var result = rejectedRelationship.EvaluateCountsAsActiveExpression();

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void CountsAsActive_with_status_Revoked()
    {
        // Arrange
        var revokedRelationship = TestData.CreateRevokedRelationship();

        // Act
        var result = revokedRelationship.EvaluateCountsAsActiveExpression();

        // Assert
        result.ShouldBeFalse();
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
        result.ShouldBeTrue();
    }

    [Fact]
    public void HasParticipant_recognizes_to_address()
    {
        // Arrange
        var relationship = TestData.CreateActiveRelationship();

        // Act
        var result = relationship.EvaluateHasParticipantExpression(relationship.To);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void HasParticipant_recognizes_foreign_addresses()
    {
        // Arrange
        var relationship = TestData.CreateActiveRelationship();

        // Act
        var result = relationship.EvaluateHasParticipantExpression("did:e:localhost:dids:1111111111111111111111");

        // Assert
        result.ShouldBeFalse();
    }

    #endregion

    #region IsBetween

    [Fact]
    public void IsBetween_returns_true_when_both_identities_are_part_of_relationship()
    {
        // Arrange
        var participant1 = CreateRandomIdentityAddress();
        var participant2 = CreateRandomIdentityAddress();
        var relationship = TestData.CreateActiveRelationship(participant1, participant2);

        // Act
        var result1 = relationship.EvaluateIsBetween(participant1, participant2);
        var result2 = relationship.EvaluateIsBetween(participant2, participant1);

        // Assert
        result1.ShouldBeTrue();
        result2.ShouldBeTrue();
    }

    [Fact]
    public void IsBetween_returns_false_when_at_least_one_identity_is_not_part_of_relationship()
    {
        // Arrange
        var participant1 = CreateRandomIdentityAddress();
        var participant2 = CreateRandomIdentityAddress();
        var otherIdentity1 = CreateRandomIdentityAddress();
        var otherIdentity2 = CreateRandomIdentityAddress();

        var relationship = TestData.CreateActiveRelationship(participant1, participant2);

        // Act
        var result1 = relationship.EvaluateIsBetween(participant1, otherIdentity1);
        var result2 = relationship.EvaluateIsBetween(participant2, otherIdentity1);
        var result3 = relationship.EvaluateIsBetween(otherIdentity1, otherIdentity2);

        // Assert
        result1.ShouldBeFalse();
        result2.ShouldBeFalse();
        result3.ShouldBeFalse();
    }

    #endregion

    #region HasStatusInWhichPeerShouldBeNotifiedAboutDeletion

    [Theory]
    [InlineData(RelationshipStatus.Pending, true)]
    [InlineData(RelationshipStatus.Rejected, false)]
    [InlineData(RelationshipStatus.Revoked, false)]
    [InlineData(RelationshipStatus.Active, true)]
    [InlineData(RelationshipStatus.Terminated, true)]
    [InlineData(RelationshipStatus.DeletionProposed, false)]
    [InlineData(RelationshipStatus.ReadyForDeletion, false)]
    public void HasStatusInWhichPeerShouldBeNotifiedAboutDeletion_with_status_Pending(RelationshipStatus status, bool expected)
    {
        // Arrange
        var relationship = TestData.CreateRelationshipInStatus(status);

        // Act
        var result = relationship.EvaluateHasStatusInWhichPeerShouldBeNotifiedAboutDeletion();

        // Assert
        result.ShouldBe(expected);
    }

    #endregion
}

file static class RelationshipExtensions
{
    extension(Relationship relationship)
    {
        public bool EvaluateHasParticipantExpression(string identity)
        {
            var expression = Relationship.HasParticipant(identity);
            return expression.Compile()(relationship);
        }

        public bool EvaluateCountsAsActiveExpression()
        {
            var expression = Relationship.CountsAsActive();
            return expression.Compile()(relationship);
        }

        public bool EvaluateHasStatusInWhichPeerShouldBeNotifiedAboutDeletion()
        {
            var expression = Relationship.HasStatusInWhichPeerShouldBeNotifiedAboutDeletion();
            return expression.Compile()(relationship);
        }

        public bool EvaluateIsBetween(IdentityAddress identity1, IdentityAddress identity2)
        {
            var expression = Relationship.IsBetween(identity1, identity2);
            return expression.Compile()(relationship);
        }
    }
}
