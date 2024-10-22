using Backbone.DevelopmentKit.Identity.ValueObjects;
using static Backbone.Modules.Relationships.Domain.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class RelationshipHasParticipantExpressionTests : AbstractTestsBase
{
    [Fact]
    public void WithParticipant_From()
    {
        // Arrange
        var identityAddress = CreateRandomIdentityAddress();
        var relationship = CreateActiveRelationship(from: identityAddress);

        // Act
        var result = relationship.HasParticipant(identityAddress);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void WithParticipant_To()
    {
        // Arrange
        var identityAddress = CreateRandomIdentityAddress();
        var relationship = CreateActiveRelationship(to: identityAddress);

        // Act
        var result = relationship.HasParticipant(identityAddress);

        // Assert
        result.Should().BeTrue();
        relationship.To.Should().Be(identityAddress);
    }

    [Fact]
    public void WithParticipant_Mixed()
    {
        // Arrange
        var identityAddressFrom = CreateRandomIdentityAddress();
        var identityAddressTo = CreateRandomIdentityAddress();
        var relationship = CreateActiveRelationship(from: identityAddressFrom, to: identityAddressTo);

        // Act
        var hasIdentityAddressFrom = relationship.HasParticipant(identityAddressFrom);
        var hasIdentityAddressTo = relationship.HasParticipant(identityAddressTo);

        // Assert
        hasIdentityAddressFrom.Should().BeTrue();
        hasIdentityAddressTo.Should().BeTrue();
        relationship.From.Should().Be(identityAddressFrom);
    }
}

#region Extensions

file static class RelationshipExtensions
{
    public static bool HasParticipant(this Relationship relationship, IdentityAddress identity)
    {
        return Relationship.HasParticipant(identity).Compile()(relationship);
    }
}

#endregion
