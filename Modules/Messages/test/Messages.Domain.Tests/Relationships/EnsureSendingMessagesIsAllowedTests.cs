using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Messages.Domain.Tests.Relationships;

public class EnsureSendingMessagesIsAllowedTests : AbstractTestsBase
{
    [Fact]
    public void Does_not_throw_if_relationship_is_pending()
    {
        // Arrange
        var relationship = CreateRelationship(RelationshipStatus.Pending);

        // Act
        var acting = () => relationship.EnsureSendingMessagesIsAllowed(CreateRandomIdentityAddress(), 0, 5);

        // Assert
        acting.ShouldNotThrow();
    }

    [Fact]
    public void Throws_if_max_number_of_unreceived_messages_is_reached()
    {
        // Arrange
        var relationship = CreateRelationship();

        // Act
        var acting = () => relationship.EnsureSendingMessagesIsAllowed(CreateRandomIdentityAddress(), 5, 5);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.message.maxNumberOfUnreceivedMessagesReached");
    }

    [Fact]
    public void Does_not_throw_if_relationship_is_terminated()
    {
        // Arrange
        var relationship = CreateRelationship(RelationshipStatus.Terminated);

        // Act
        var acting = () => relationship.EnsureSendingMessagesIsAllowed(CreateRandomIdentityAddress(), 0, 5);

        // Assert
        acting.ShouldNotThrow();
    }

    #region helpers

    private static Relationship CreateRelationship(RelationshipStatus status)
    {
        return CreateRelationship(null, null, null, null, status);
    }

    private static Relationship CreateRelationship(string? relationshipId = null, IdentityAddress? from = null, IdentityAddress? to = null, DateTime? createdAt = null,
        RelationshipStatus? status = null)
    {
        relationshipId ??= "REL00000000000000000";
        from ??= CreateRandomIdentityAddress();
        to ??= CreateRandomIdentityAddress();
        createdAt ??= DateTime.UtcNow;
        status ??= RelationshipStatus.Active;

        return Relationship.LoadForTesting(RelationshipId.Parse(relationshipId), from, to, createdAt.Value, status.Value);
    }

    #endregion
}
