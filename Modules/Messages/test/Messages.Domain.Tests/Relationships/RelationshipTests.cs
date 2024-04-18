using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.UnitTestTools.Data;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Messages.Domain.Tests.Relationships;
public class RelationshipTests
{
    [Fact]
    public void Relationship_must_be_active_to_allow_sending_messages()
    {
        // Arrange
        var relationship = CreateRelationship(RelationshipStatus.Pending);

        // Act
        var acting = () => relationship.EnsureSendingMessagesIsAllowed(0, 5);

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.message.relationshipToRecipientNotActive");
    }

    [Fact]
    public void Max_number_of_unreceived_messages_must_not_be_reached()
    {
        // Arrange
        var relationship = CreateRelationship();

        // Act
        var acting = () => relationship.EnsureSendingMessagesIsAllowed(5, 5);

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.message.maxNumberOfUnreceivedMessagesReached");
    }

    [Fact]
    public void Relationship_cannot_be_terminated_to_allow_sending_messages()
    {
        // Arrange
        var relationship = CreateRelationship(RelationshipStatus.Terminated);

        // Act
        var acting = () => relationship.EnsureSendingMessagesIsAllowed(0, 5);

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.message.relationshipToRecipientNotActive");
    }

    #region helpers

    private static Relationship CreateRelationship(RelationshipStatus status)
    {
        return CreateRelationship(null, null, null, null, status);
    }

    private static Relationship CreateRelationship(string? relationshipId = null, IdentityAddress? from = null, IdentityAddress? to = null, DateTime? createdAt = null, RelationshipStatus? status = null)
    {
        relationshipId ??= "REL00000000000000000";
        from ??= TestDataGenerator.CreateRandomIdentityAddress();
        to ??= TestDataGenerator.CreateRandomIdentityAddress();
        createdAt ??= DateTime.UtcNow;
        status ??= RelationshipStatus.Active;

        return Relationship.LoadForTesting(RelationshipId.Parse(relationshipId), from, to, createdAt.Value, status.Value);
    }
    #endregion
}
