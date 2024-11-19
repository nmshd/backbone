using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.UnitTestTools.Extensions;
using static Backbone.Modules.Relationships.Domain.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class RelationshipDecomposeDueToIdentityDeletionTests : AbstractTestsBase
{
    private const string DID_DOMAIN_NAME = "localhost";

    [Theory]
    [InlineData(RelationshipStatus.Pending)]
    [InlineData(RelationshipStatus.Active)]
    [InlineData(RelationshipStatus.Rejected)]
    [InlineData(RelationshipStatus.Revoked)]
    [InlineData(RelationshipStatus.Terminated)]
    public void Decomposition_can_be_performed_from_multiple_statuses(RelationshipStatus status)
    {
        // Arrange
        var relationship = CreateRelationshipInStatus(status, IDENTITY_1, IDENTITY_2);

        // Act
        var acting = () => relationship.DecomposeDueToIdentityDeletion(IDENTITY_1, DID_DOMAIN_NAME);

        // Assert
        acting.Should().NotThrow();
        relationship.Status.Should().Be(RelationshipStatus.DeletionProposed);
        relationship.FromHasDecomposed.Should().BeTrue();
        relationship.ToHasDecomposed.Should().BeFalse();
    }

    [Fact]
    public void Decomposition_can_not_be_called_by_the_same_identity_twice()
    {
        // Arrange
        var relationship = CreateRelationshipDecomposedByFrom(IDENTITY_1, IDENTITY_2);

        // Act
        var acting = () => relationship.DecomposeDueToIdentityDeletion(IDENTITY_1, DID_DOMAIN_NAME);

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationship.relationshipAlreadyDecomposed");
    }

    [Fact]
    public void Decomposition_can_not_be_performed_by_other_identities()
    {
        // Arrange
        var relationship = CreateActiveRelationship(IDENTITY_1, IDENTITY_2);

        // Act
        var acting = () => relationship.DecomposeDueToIdentityDeletion(CreateRandomIdentityAddress(), DID_DOMAIN_NAME);

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationship.requestingIdentityDoesNotBelongToRelationship");
    }

    [Fact]
    public void Decomposition_by_both_identities_transitions_relationship_to_status_ReadyForDeletion()
    {
        // Arrange
        var relationship = CreateActiveRelationship(IDENTITY_1, IDENTITY_2);
        relationship.DecomposeDueToIdentityDeletion(IDENTITY_1, DID_DOMAIN_NAME);

        // Act
        var acting = () => relationship.DecomposeDueToIdentityDeletion(IDENTITY_2, DID_DOMAIN_NAME);

        // Assert
        acting.Should().NotThrow();
        relationship.Status.Should().Be(RelationshipStatus.ReadyForDeletion);
    }

    [Fact]
    public void Anonymize_anonymizes_the_audit_logs()
    {
        // Arrange
        var relationship = CreateActiveRelationship(IDENTITY_1, IDENTITY_2);
        var anonymousIdentity = IdentityAddress.GetAnonymized(DID_DOMAIN_NAME);

        // Act
        relationship.DecomposeDueToIdentityDeletion(IDENTITY_1, DID_DOMAIN_NAME);

        // Assert
        relationship.AuditLog.Should().HaveCount(3);
        relationship.AuditLog[0].CreatedBy.Should().Be(anonymousIdentity);
        relationship.AuditLog[2].CreatedBy.Should().Be(anonymousIdentity);
    }
}
