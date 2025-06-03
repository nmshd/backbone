using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.UnitTestTools.Shouldly.Extensions;
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
        acting.ShouldNotThrow();
        relationship.Status.ShouldBe(RelationshipStatus.DeletionProposed);
        relationship.FromHasDecomposed.ShouldBeTrue();
        relationship.ToHasDecomposed.ShouldBeFalse();
    }

    [Fact]
    public void Decomposition_can_not_be_performed_by_other_identities()
    {
        // Arrange
        var relationship = CreateActiveRelationship(IDENTITY_1, IDENTITY_2);

        // Act
        var acting = () => relationship.DecomposeDueToIdentityDeletion(CreateRandomIdentityAddress(), DID_DOMAIN_NAME);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.relationship.requestingIdentityDoesNotBelongToRelationship");
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
        acting.ShouldNotThrow();
        relationship.Status.ShouldBe(RelationshipStatus.ReadyForDeletion);
    }

    [Fact]
    public void Anonymizes_the_deleted_identity()
    {
        // Arrange
        var relationship = CreateActiveRelationship(IDENTITY_1, IDENTITY_2);
        var anonymousIdentity = IdentityAddress.GetAnonymized(DID_DOMAIN_NAME);

        // Act
        relationship.DecomposeDueToIdentityDeletion(IDENTITY_1, DID_DOMAIN_NAME);

        // Assert
        relationship.From.ShouldBe(anonymousIdentity);
        relationship.FromHasDecomposed.ShouldBeTrue();
    }

    [Fact]
    public void Anonymizes_the_audit_logs()
    {
        // Arrange
        var relationship = CreateActiveRelationship(IDENTITY_1, IDENTITY_2);
        var anonymousIdentity = IdentityAddress.GetAnonymized(DID_DOMAIN_NAME);

        // Act
        relationship.DecomposeDueToIdentityDeletion(IDENTITY_1, DID_DOMAIN_NAME);

        // Assert
        relationship.AuditLog.ShouldHaveCount(3);
        relationship.AuditLog[0].CreatedBy.ShouldBe(anonymousIdentity);
        relationship.AuditLog[2].CreatedBy.ShouldBe(anonymousIdentity);
    }
}
