using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Tests.TestHelpers;
using Xunit;

namespace Backbone.Modules.Relationships.Domain.Tests.Tests.Aggregates.Relationships;

public class ExpressionTests
{
    #region CountsAsActive

    [Fact]
    public void CountsAsActive_with_status_Pending()
    {
        var pendingRelationship = TestData.CreatePendingRelationship();

        var result = pendingRelationship.EvaluateCountsAsActiveExpression();

        Assert.True(result);
    }

    [Fact]
    public void CountsAsActive_with_status_Active()
    {
        var activeRelationship = TestData.CreateActiveRelationship();

        var result = activeRelationship.EvaluateCountsAsActiveExpression();

        Assert.True(result);
    }

    [Fact]
    public void CountsAsActive_with_status_Rejected()
    {
        var rejectedRelationship = TestData.CreateRejectedRelationship();

        var result = rejectedRelationship.EvaluateCountsAsActiveExpression();

        Assert.False(result);
    }

    [Fact]
    public void CountsAsActive_with_status_Revoked()
    {
        var revokedRelationship = TestData.CreateRevokedRelationship();

        var result = revokedRelationship.EvaluateCountsAsActiveExpression();

        Assert.False(result);
    }

    #endregion

    #region HasParticipant

    [Fact]
    public void HasParticipant_recognizes_from_address()
    {
        var relationship = TestData.CreateActiveRelationship();

        var result = relationship.EvaluateHasParticipantExpression(relationship.From);

        Assert.True(result);
    }

    [Fact]
    public void HasParticipant_recognizes_to_address()
    {
        var relationship = TestData.CreateActiveRelationship();

        var result = relationship.EvaluateHasParticipantExpression(relationship.To);

        Assert.True(result);
    }

    [Fact]
    public void HasParticipant_recognizes_foreign_addresses()
    {
        var relationship = TestData.CreateActiveRelationship();

        var result = relationship.EvaluateHasParticipantExpression("id1");

        Assert.False(result);
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
