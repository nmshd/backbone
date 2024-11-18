using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.UnitTestTools.Extensions;
using static Backbone.Modules.Relationships.Domain.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class RelationshipAnonymizeTests : AbstractTestsBase
{
    private const string DID_DOMAIN_NAME = "localhost";

    [Theory]
    [InlineData(RelationshipStatus.DeletionProposed)]
    [InlineData(RelationshipStatus.ReadyForDeletion)]
    public void Anonymize_can_be_performed_for_decomposed_relationship(RelationshipStatus status)
    {
        // Arrange
        var relationship = CreateRelationshipInStatus(status, IDENTITY_1, IDENTITY_2);

        // Act
        var acting = () => relationship.AnonymizeParticipant(IDENTITY_1, DID_DOMAIN_NAME);

        // Assert
        acting.Should().NotThrow();
        relationship.From.Should().Be(IdentityAddress.GetAnonymized(DID_DOMAIN_NAME));
    }

    [Fact]
    public void Anonymize_can_be_performed_for_other_participant()
    {
        // Arrange
        var relationship = CreateRelationshipReadyForDeletion(IDENTITY_1, IDENTITY_2);

        // Act
        var acting = () => relationship.AnonymizeParticipant(IDENTITY_2, DID_DOMAIN_NAME);

        // Assert
        acting.Should().NotThrow();
        relationship.To.Should().Be(IdentityAddress.GetAnonymized(DID_DOMAIN_NAME));
    }

    [Fact]
    public void Anonymize_can_not_be_performed_by_other_identities()
    {
        // Arrange
        var relationship = CreateRelationshipReadyForDeletion(IDENTITY_1, IDENTITY_2);

        // Act
        var acting = () => relationship.AnonymizeParticipant(CreateRandomIdentityAddress(), DID_DOMAIN_NAME);

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationship.requestingIdentityDoesNotBelongToRelationship");
    }

    [Theory]
    [InlineData(RelationshipStatus.Pending)]
    [InlineData(RelationshipStatus.Active)]
    [InlineData(RelationshipStatus.Rejected)]
    [InlineData(RelationshipStatus.Revoked)]
    [InlineData(RelationshipStatus.Terminated)]
    public void Anonymize_can_not_be_performed_when_not_decomposed(RelationshipStatus status)
    {
        // Arrange
        var relationship = CreateRelationshipInStatus(status, IDENTITY_1, IDENTITY_2);

        // Act
        var acting = () => relationship.AnonymizeParticipant(IDENTITY_1, DID_DOMAIN_NAME);

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationship.relationshipIsNotInCorrectStatus");
    }

    [Fact]
    public void Anonymize_anonymizes_the_audit_logs()
    {
        // Arrange
        var relationship = CreateActiveRelationship(IDENTITY_1, IDENTITY_2);
        relationship.DecomposeDueToIdentityDeletion(IDENTITY_1);
        var anonymousIdentity = IdentityAddress.GetAnonymized(DID_DOMAIN_NAME);

        // Act
        relationship.AnonymizeParticipant(IDENTITY_1, DID_DOMAIN_NAME);

        // Assert
        relationship.AuditLog.Should().HaveCount(3);
        relationship.AuditLog[0].CreatedBy.Should().Be(anonymousIdentity);
        relationship.AuditLog[2].CreatedBy.Should().Be(anonymousIdentity);
    }
}
