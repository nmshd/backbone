using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Relationships.Domain.Tests.Tests;

public class RelationshipTests
{
    private static readonly IdentityAddress FROM_IDENTITY = IdentityAddress.Create([1, 1, 1], "id1");

    private static readonly DeviceId FROM_DEVICE = DeviceId.New();

//
    private static readonly IdentityAddress TO_IDENTITY = IdentityAddress.Create([2, 2, 2], "id1");

    private static readonly DeviceId TO_DEVICE = DeviceId.New();

//
//     private static readonly byte[] REQUEST_CONTENT = [1, 1, 1];
//     private static readonly byte[] RESPONSE_CONTENT = [2, 2, 2];
//
//     private static readonly RelationshipTemplate TEMPLATE = new(TO_IDENTITY, TO_DEVICE, 1, SystemTime.UtcNow.AddDays(1), [0]);
//
//     #region Creation
//
    [Fact]
    public void New_Relationship_Has_Correct_Data()
    {
        // Arrange
        var relationshipTemplate = new RelationshipTemplate(TO_IDENTITY, TO_DEVICE, 1, null, []);
        SystemTime.Set("2000-01-01");

        // Act
        var relationship = new Relationship(relationshipTemplate, FROM_IDENTITY, FROM_DEVICE, null);

        // Assert
        relationship.From.Should().Be(FROM_IDENTITY);
        relationship.To.Should().Be(TO_IDENTITY);
        relationship.Status.Should().Be(RelationshipStatus.Pending);

        relationship.AuditLog.Should().HaveCount(1);
        var auditLogEntry = relationship.AuditLog.First();
        auditLogEntry.Id.Should().NotBeNull();
        auditLogEntry.Reason.Should().Be(RelationshipAuditLogEntryReason.Creation);
        auditLogEntry.OldStatus.Should().Be(null);
        auditLogEntry.NewStatus.Should().Be(RelationshipStatus.Pending);
        auditLogEntry.CreatedBy.Should().Be(FROM_IDENTITY);
        auditLogEntry.CreatedByDevice.Should().Be(FROM_DEVICE);
        auditLogEntry.CreatedAt.Should().Be(DateTime.Parse("2000-01-01"));
    }
//
//     #region Accept Creation
//
//     [Fact]
//     public void Accepting_CreationRequest_Changes_Relevant_Data()
//     {
//         var relationship = CreatePendingRelationship();
//         var change = relationship.Changes.GetOpenCreation()!;
//
//         relationship.AcceptChange(change.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);
//
//         relationship.Status.Should().Be(RelationshipStatus.Active);
//
//         change.Status.Should().Be(RelationshipChangeStatus.Accepted);
//
//         change.Response.Should().NotBeNull();
//         change.Response!.Content.Should().Equal(RESPONSE_CONTENT);
//         change.Response!.CreatedBy.Should().Be(TO_IDENTITY);
//         change.Response!.CreatedByDevice.Should().Be(TO_DEVICE);
//     }
//
//     [Fact]
//     public void Cannot_Accept_CreationRequests_Without_Content()
//     {
//         var relationship = CreatePendingRelationship();
//         var change = relationship.Changes.GetOpenCreation()!;
//
//         Action acting = () => relationship.AcceptChange(change.Id, TO_IDENTITY, TO_DEVICE, null);
//         acting.Should().Throw<DomainException>().WithError(DomainErrors.ContentIsRequiredForCompletingRelationships());
//     }
//
//     [Fact]
//     public void Cannot_Accept_Already_Completed_CreationRequests()
//     {
//         var relationship = CreatePendingRelationship();
//         var change = relationship.Changes.GetOpenCreation()!;
//         relationship.AcceptChange(change.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);
//
//         Action acting = () => relationship.AcceptChange(change.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);
//         acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestIsAlreadyCompleted());
//     }
//
//     [Fact]
//     public void Relationship_Cannot_Be_Accepted_By_Creator()
//     {
//         var relationship = CreatePendingRelationship();
//         var change = relationship.Changes.GetOpenCreation()!;
//
//         Action acting = () => relationship.AcceptChange(change.Id, FROM_IDENTITY, FROM_DEVICE, RESPONSE_CONTENT);
//         acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestCannotBeAcceptedByCreator());
//     }
//
//     #endregion
//
//     #region Reject Creation
//
//     [Fact]
//     public void Rejecting_CreationRequest_Sets_Relevant_Data()
//     {
//         var relationship = CreatePendingRelationship();
//         var change = relationship.Changes.GetOpenCreation()!;
//
//         relationship.RejectChange(change.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);
//
//         relationship.Status.Should().Be(RelationshipStatus.Rejected);
//
//         change.Status.Should().Be(RelationshipChangeStatus.Rejected);
//
//         change.Response.Should().NotBeNull();
//         change.Response!.Content.Should().Equal(RESPONSE_CONTENT);
//         change.Response!.CreatedBy.Should().Be(TO_IDENTITY);
//         change.Response!.CreatedByDevice.Should().Be(TO_DEVICE);
//     }
//
//     [Fact]
//     public void Cannot_Reject_CreationRequests_Without_Content()
//     {
//         var relationship = CreatePendingRelationship();
//         var change = relationship.Changes.GetOpenCreation()!;
//
//         Action acting = () => relationship.RejectChange(change.Id, TO_IDENTITY, TO_DEVICE, null);
//         acting.Should().Throw<DomainException>().WithError(DomainErrors.ContentIsRequiredForCompletingRelationships());
//     }
//
//     [Fact]
//     public void Cannot_Reject_Already_Completed_CreationRequests()
//     {
//         var relationship = CreatePendingRelationship();
//         var change = relationship.Changes.GetOpenCreation()!;
//         relationship.RejectChange(change.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);
//
//         Action acting = () => relationship.RejectChange(change.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);
//         acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestIsAlreadyCompleted());
//     }
//
//     [Fact]
//     public void CreationRequest_Cannot_Be_Rejected_By_Creator()
//     {
//         var relationship = CreatePendingRelationship();
//         var change = relationship.Changes.GetOpenCreation()!;
//
//         Action acting = () => relationship.RejectChange(change.Id, FROM_IDENTITY, FROM_DEVICE, RESPONSE_CONTENT);
//         acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestCannotBeRejectedByCreator());
//     }
//
//     #endregion
//
//     #region Revoke Creation
//
//     [Fact]
//     public void Revoking_CreationRequest_Sets_Relevant_Data()
//     {
//         var relationship = CreatePendingRelationship();
//         var change = relationship.Changes.GetOpenCreation()!;
//
//         relationship.RevokeChange(change.Id, FROM_IDENTITY, FROM_DEVICE, RESPONSE_CONTENT);
//
//         relationship.Status.Should().Be(RelationshipStatus.Revoked);
//
//         change.Status.Should().Be(RelationshipChangeStatus.Revoked);
//
//         change.Response.Should().NotBeNull();
//         change.Response!.Content.Should().Equal(RESPONSE_CONTENT);
//         change.Response!.CreatedBy.Should().Be(FROM_IDENTITY);
//         change.Response!.CreatedByDevice.Should().Be(FROM_DEVICE);
//     }
//
//     [Fact]
//     public void Cannot_Revoke_CreationRequests_Without_Content()
//     {
//         var relationship = CreatePendingRelationship();
//         var change = relationship.Changes.GetOpenCreation()!;
//
//         Action acting = () => relationship.RevokeChange(change.Id, FROM_IDENTITY, FROM_DEVICE, null);
//         acting.Should().Throw<DomainException>().WithError(DomainErrors.ContentIsRequiredForCompletingRelationships());
//     }
//
//     [Fact]
//     public void Cannot_Revoke_Already_Completed_CreationRequests()
//     {
//         var relationship = CreatePendingRelationship();
//         var change = relationship.Changes.GetOpenCreation()!;
//         relationship.RevokeChange(change.Id, FROM_IDENTITY, FROM_DEVICE, RESPONSE_CONTENT);
//
//         Action acting = () => relationship.RevokeChange(change.Id, FROM_IDENTITY, FROM_DEVICE, RESPONSE_CONTENT);
//         acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestIsAlreadyCompleted());
//     }
//
//     [Fact]
//     public void CreationRequest_Cannot_Be_Revoked_By_Recipient()
//     {
//         var relationship = CreatePendingRelationship();
//         var change = relationship.Changes.GetOpenCreation()!;
//
//         Action acting = () => relationship.RevokeChange(change.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);
//         acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestCanOnlyBeRevokedByCreator());
//     }
//
//     #endregion
//
//     #endregion
//
//     #region Common
//
//     [Fact]
//     public void Cannot_Accept_Non_Existent_ChangeRequests()
//     {
//         var relationship = CreatePendingRelationship();
//
//         Action acting = () => relationship.AcceptChange(RelationshipChangeId.New(), TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);
//         acting.Should().Throw<DomainException>().WithError(GenericDomainErrors.NotFound());
//     }
//
//     [Fact]
//     public void Cannot_Reject_Non_Existent_ChangeRequests()
//     {
//         var relationship = CreatePendingRelationship();
//
//         Action acting = () => relationship.RejectChange(RelationshipChangeId.New(), TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);
//         acting.Should().Throw<DomainException>().WithError(GenericDomainErrors.NotFound());
//     }
//
//     [Fact]
//     public void Cannot_Revoke_Non_Existent_ChangeRequests()
//     {
//         var relationship = CreatePendingRelationship();
//
//         Action acting = () => relationship.RevokeChange(RelationshipChangeId.New(), TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);
//         acting.Should().Throw<DomainException>().WithError(GenericDomainErrors.NotFound());
//     }
//
//     #endregion
//
//     #region Termination
//
//     [Fact]
//     public void Requesting_Termination_Sets_Relevant_Data()
//     {
//         var relationship = CreateActiveRelationship();
//
//         relationship.RequestTermination(FROM_IDENTITY, FROM_DEVICE);
//
//         relationship.Changes.Should().HaveCount(2);
//
//         var termination = relationship.Changes.GetOpenTermination()!;
//         termination.Should().NotBeNull();
//         termination.Status.Should().Be(RelationshipChangeStatus.Pending);
//         termination.Request.Should().NotBeNull();
//         termination.Request.CreatedBy.Should().Be(FROM_IDENTITY);
//         termination.Request.CreatedByDevice.Should().Be(FROM_DEVICE);
//
//         termination.Response.Should().BeNull();
//     }
//
//     [Fact]
//     public void Cannot_Request_Termination_For_Pending_Relationships()
//     {
//         var relationship = CreatePendingRelationship();
//
//         Action acting = () => relationship.RequestTermination(FROM_IDENTITY, FROM_DEVICE);
//         acting.Should().Throw<DomainException>().WithError(DomainErrors.OnlyActiveRelationshipsCanBeTerminated());
//     }
//
//     #region Accept Termination
//
//     [Fact]
//     public void Accepting_TerminationRequest_Changes_Relevant_Data()
//     {
//         var relationship = CreateRelationshipWithOpenTermination();
//         var change = relationship.Changes.GetOpenTermination()!;
//
//         relationship.AcceptChange(change.Id, TO_IDENTITY, TO_DEVICE, null);
//
//         relationship.Status.Should().Be(RelationshipStatus.Terminated);
//
//         change.Status.Should().Be(RelationshipChangeStatus.Accepted);
//
//         change.Response.Should().NotBeNull();
//         change.Response!.CreatedBy.Should().Be(TO_IDENTITY);
//         change.Response!.CreatedByDevice.Should().Be(TO_DEVICE);
//     }
//
//     [Fact]
//     public void Cannot_Accept_Already_Completed_TerminationRequests()
//     {
//         var relationship = CreateRelationshipWithOpenTermination();
//         var change = relationship.Changes.GetOpenTermination()!;
//
//         relationship.AcceptChange(change.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);
//
//         Action acting = () => relationship.AcceptChange(change.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);
//         acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestIsAlreadyCompleted());
//     }
//
//     [Fact]
//     public void TerminationRequest_Cannot_Be_Accepted_By_Creator()
//     {
//         var relationship = CreateRelationshipWithOpenTermination();
//         var change = relationship.Changes.GetOpenTermination()!;
//
//         Action acting = () => relationship.AcceptChange(change.Id, FROM_IDENTITY, FROM_DEVICE, null);
//         acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestCannotBeAcceptedByCreator());
//     }
//
//     #endregion
//
//     #region Reject Termination
//
//     [Fact]
//     public void Rejecting_TerminationRequest_Changes_Relevant_Data()
//     {
//         var relationship = CreateRelationshipWithOpenTermination();
//         var change = relationship.Changes.GetOpenTermination()!;
//
//         relationship.RejectChange(change.Id, TO_IDENTITY, TO_DEVICE, null);
//
//         relationship.Status.Should().Be(RelationshipStatus.Active);
//
//         change.Status.Should().Be(RelationshipChangeStatus.Rejected);
//
//         change.Response.Should().NotBeNull();
//         change.Response!.CreatedBy.Should().Be(TO_IDENTITY);
//         change.Response!.CreatedByDevice.Should().Be(TO_DEVICE);
//     }
//
//     [Fact]
//     public void Cannot_Reject_Already_Completed_TerminationRequests()
//     {
//         var relationship = CreateRelationshipWithOpenTermination();
//         var change = relationship.Changes.GetOpenTermination()!;
//
//         relationship.RejectChange(change.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);
//
//         Action acting = () => relationship.RejectChange(change.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);
//         acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestIsAlreadyCompleted());
//     }
//
//     [Fact]
//     public void TerminationRequest_Cannot_Be_Rejected_By_Creator()
//     {
//         var relationship = CreateRelationshipWithOpenTermination();
//         var change = relationship.Changes.GetOpenTermination()!;
//
//         Action acting = () => relationship.RejectChange(change.Id, FROM_IDENTITY, FROM_DEVICE, null);
//         acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestCannotBeRejectedByCreator());
//     }
//
//     #endregion
//
//     #region Revoke Termination
//
//     [Fact]
//     public void Revoking_TerminationRequest_Sets_Relevant_Data()
//     {
//         var relationship = CreateRelationshipWithOpenTermination();
//         var change = relationship.Changes.GetOpenTermination()!;
//
//         relationship.RevokeChange(change.Id, FROM_IDENTITY, FROM_DEVICE, null);
//
//         relationship.Status.Should().Be(RelationshipStatus.Active);
//
//         change.Status.Should().Be(RelationshipChangeStatus.Revoked);
//
//         change.Response.Should().NotBeNull();
//         change.Response!.CreatedBy.Should().Be(FROM_IDENTITY);
//         change.Response!.CreatedByDevice.Should().Be(FROM_DEVICE);
//     }
//
//     [Fact]
//     public void Cannot_Revoke_Already_Completed_TerminationRequests()
//     {
//         var relationship = CreateRelationshipWithOpenTermination();
//         var change = relationship.Changes.GetOpenTermination()!;
//
//         relationship.RevokeChange(change.Id, FROM_IDENTITY, FROM_DEVICE, RESPONSE_CONTENT);
//
//         Action acting = () => relationship.RevokeChange(change.Id, FROM_IDENTITY, FROM_DEVICE, RESPONSE_CONTENT);
//         acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestIsAlreadyCompleted());
//     }
//
//     [Fact]
//     public void TerminationRequest_Cannot_Be_Revoked_By_Recipient()
//     {
//         var relationship = CreateRelationshipWithOpenTermination();
//         var change = relationship.Changes.GetOpenTermination()!;
//
//         Action acting = () => relationship.RevokeChange(change.Id, TO_IDENTITY, TO_DEVICE, null);
//         acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestCanOnlyBeRevokedByCreator());
//     }
//
//     #endregion
//
//     #endregion
//
//     #region Constructor Functions
//
//     private static Relationship CreatePendingRelationship()
//     {
//         var relationship = new Relationship(TEMPLATE, FROM_IDENTITY, FROM_DEVICE, REQUEST_CONTENT);
//         return relationship;
//     }
//
//     private static Relationship CreateActiveRelationship()
//     {
//         var relationship = new Relationship(TEMPLATE, FROM_IDENTITY, FROM_DEVICE, REQUEST_CONTENT);
//         var change = relationship.Changes.GetOpenCreation();
//         relationship.AcceptChange(change!.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);
//         return relationship;
//     }
//
//     private static Relationship CreateRelationshipWithOpenTermination()
//     {
//         var relationship = new Relationship(TEMPLATE, FROM_IDENTITY, FROM_DEVICE, REQUEST_CONTENT);
//         var change = relationship.Changes.GetOpenCreation();
//         relationship.AcceptChange(change!.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);
//
//         relationship.RequestTermination(FROM_IDENTITY, FROM_DEVICE);
//         return relationship;
//     }
//
//     #endregion
}
