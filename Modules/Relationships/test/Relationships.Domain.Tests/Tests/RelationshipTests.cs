using Backbone.Modules.Relationships.Domain;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Errors;
using Backbone.Modules.Relationships.Domain.Ids;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;
using FluentAssertions;
using Relationships.Domain.Tests.Extensions;
using Xunit;

namespace Relationships.Domain.Tests.Tests;

public class RelationshipTests
{
    private static readonly IdentityAddress FromIdentity = IdentityAddress.Create(new byte[] { 1, 1, 1 }, "id1");
    private static readonly DeviceId FromDevice = DeviceId.New();

    private static readonly IdentityAddress ToIdentity = IdentityAddress.Create(new byte[] { 2, 2, 2 }, "id1");
    private static readonly DeviceId ToDevice = DeviceId.New();

    private static readonly byte[] RequestContent = { 1, 1, 1 };
    private static readonly byte[] ResponseContent = { 2, 2, 2 };

    private static readonly RelationshipTemplate Template = new(ToIdentity, ToDevice, 1, SystemTime.UtcNow.AddDays(1), new byte[] { 0 });

    #region Creation

    [Fact]
    public void New_Relationship_Has_Correct_Data()
    {
        var relationship = CreatePendingRelationship();

        relationship.From.Should().Be(FromIdentity);
        relationship.To.Should().Be(ToIdentity);
        relationship.Status.Should().Be(RelationshipStatus.Pending);

        relationship.Changes.Should().HaveCount(1);
        var change = relationship.Changes.GetLatestOfType(RelationshipChangeType.Creation);
        change.Request.CreatedBy.Should().Be(FromIdentity);
        change.Request.CreatedByDevice.Should().Be(FromDevice);

        change.Type.Should().Be(RelationshipChangeType.Creation);
        change.Status.Should().Be(RelationshipChangeStatus.Pending);

        change.Response.Should().BeNull();
    }

    #region Accept Creation

    [Fact]
    public void Accepting_CreationRequest_Changes_Relevant_Data()
    {
        var relationship = CreatePendingRelationship();
        var change = relationship.Changes.GetOpenCreation();

        relationship.AcceptChange(change.Id, ToIdentity, ToDevice, ResponseContent);

        relationship.Status.Should().Be(RelationshipStatus.Active);

        change.Status.Should().Be(RelationshipChangeStatus.Accepted);

        change.Response.Should().NotBeNull();
        change.Response!.Content.Should().Equal(ResponseContent);
        change.Response!.CreatedBy.Should().Be(ToIdentity);
        change.Response!.CreatedByDevice.Should().Be(ToDevice);
    }

    [Fact]
    public void Cannot_Accept_CreationRequests_Without_Content()
    {
        var relationship = CreatePendingRelationship();
        var change = relationship.Changes.GetOpenCreation();

        Action acting = () => relationship.AcceptChange(change.Id, ToIdentity, ToDevice, null);
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ContentIsRequiredForCompletingRelationships());
    }

    [Fact]
    public void Cannot_Accept_Already_Completed_CreationRequests()
    {
        var relationship = CreatePendingRelationship();
        var change = relationship.Changes.GetOpenCreation();
        relationship.AcceptChange(change.Id, ToIdentity, ToDevice, ResponseContent);

        Action acting = () => relationship.AcceptChange(change.Id, ToIdentity, ToDevice, ResponseContent);
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestIsAlreadyCompleted());
    }

    [Fact]
    public void Relationship_Cannot_Be_Accepted_By_Creator()
    {
        var relationship = CreatePendingRelationship();
        var change = relationship.Changes.GetOpenCreation();

        Action acting = () => relationship.AcceptChange(change.Id, FromIdentity, FromDevice, ResponseContent);
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestCannotBeAcceptedByCreator());
    }

    #endregion

    #region Reject Creation

    [Fact]
    public void Rejecting_CreationRequest_Sets_Relevant_Data()
    {
        var relationship = CreatePendingRelationship();
        var change = relationship.Changes.GetOpenCreation();

        relationship.RejectChange(change.Id, ToIdentity, ToDevice, ResponseContent);

        relationship.Status.Should().Be(RelationshipStatus.Rejected);

        change.Status.Should().Be(RelationshipChangeStatus.Rejected);

        change.Response.Should().NotBeNull();
        change.Response!.Content.Should().Equal(ResponseContent);
        change.Response!.CreatedBy.Should().Be(ToIdentity);
        change.Response!.CreatedByDevice.Should().Be(ToDevice);
    }

    [Fact]
    public void Cannot_Reject_CreationRequests_Without_Content()
    {
        var relationship = CreatePendingRelationship();
        var change = relationship.Changes.GetOpenCreation();

        Action acting = () => relationship.RejectChange(change.Id, ToIdentity, ToDevice, null);
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ContentIsRequiredForCompletingRelationships());
    }

    [Fact]
    public void Cannot_Reject_Already_Completed_CreationRequests()
    {
        var relationship = CreatePendingRelationship();
        var change = relationship.Changes.GetOpenCreation();
        relationship.RejectChange(change.Id, ToIdentity, ToDevice, ResponseContent);

        Action acting = () => relationship.RejectChange(change.Id, ToIdentity, ToDevice, ResponseContent);
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestIsAlreadyCompleted());
    }

    [Fact]
    public void CreationRequest_Cannot_Be_Rejected_By_Creator()
    {
        var relationship = CreatePendingRelationship();
        var change = relationship.Changes.GetOpenCreation();

        Action acting = () => relationship.RejectChange(change.Id, FromIdentity, FromDevice, ResponseContent);
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestCannotBeRejectedByCreator());
    }

    #endregion

    #region Revoke Creation

    [Fact]
    public void Revoking_CreationRequest_Sets_Relevant_Data()
    {
        var relationship = CreatePendingRelationship();
        var change = relationship.Changes.GetOpenCreation();

        relationship.RevokeChange(change.Id, FromIdentity, FromDevice, ResponseContent);

        relationship.Status.Should().Be(RelationshipStatus.Revoked);

        change.Status.Should().Be(RelationshipChangeStatus.Revoked);

        change.Response.Should().NotBeNull();
        change.Response!.Content.Should().Equal(ResponseContent);
        change.Response!.CreatedBy.Should().Be(FromIdentity);
        change.Response!.CreatedByDevice.Should().Be(FromDevice);
    }

    [Fact]
    public void Cannot_Revoke_CreationRequests_Without_Content()
    {
        var relationship = CreatePendingRelationship();
        var change = relationship.Changes.GetOpenCreation();

        Action acting = () => relationship.RevokeChange(change.Id, FromIdentity, FromDevice, null);
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ContentIsRequiredForCompletingRelationships());
    }

    [Fact]
    public void Cannot_Revoke_Already_Completed_CreationRequests()
    {
        var relationship = CreatePendingRelationship();
        var change = relationship.Changes.GetOpenCreation();
        relationship.RevokeChange(change.Id, FromIdentity, FromDevice, ResponseContent);

        Action acting = () => relationship.RevokeChange(change.Id, FromIdentity, FromDevice, ResponseContent);
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestIsAlreadyCompleted());
    }

    [Fact]
    public void CreationRequest_Cannot_Be_Revoked_By_Recipient()
    {
        var relationship = CreatePendingRelationship();
        var change = relationship.Changes.GetOpenCreation();

        Action acting = () => relationship.RevokeChange(change.Id, ToIdentity, ToDevice, ResponseContent);
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestCanOnlyBeRevokedByCreator());
    }

    #endregion

    #endregion

    #region Common

    [Fact]
    public void Cannot_Accept_Non_Existent_ChangeRequests()
    {
        var relationship = CreatePendingRelationship();

        Action acting = () => relationship.AcceptChange(RelationshipChangeId.New(), ToIdentity, ToDevice, ResponseContent);
        acting.Should().Throw<DomainException>().WithError(DomainErrors.NotFound());
    }

    [Fact]
    public void Cannot_Reject_Non_Existent_ChangeRequests()
    {
        var relationship = CreatePendingRelationship();

        Action acting = () => relationship.RejectChange(RelationshipChangeId.New(), ToIdentity, ToDevice, ResponseContent);
        acting.Should().Throw<DomainException>().WithError(DomainErrors.NotFound());
    }

    [Fact]
    public void Cannot_Revoke_Non_Existent_ChangeRequests()
    {
        var relationship = CreatePendingRelationship();

        Action acting = () => relationship.RevokeChange(RelationshipChangeId.New(), ToIdentity, ToDevice, ResponseContent);
        acting.Should().Throw<DomainException>().WithError(DomainErrors.NotFound());
    }

    #endregion

    #region Termination

    [Fact]
    public void Requesting_Termination_Sets_Relevant_Data()
    {
        var relationship = CreateActiveRelationship();

        relationship.RequestTermination(FromIdentity, FromDevice);

        relationship.Changes.Should().HaveCount(2);

        var termination = relationship.Changes.GetOpenTermination();
        termination.Should().NotBeNull();
        termination.Status.Should().Be(RelationshipChangeStatus.Pending);
        termination.Request.Should().NotBeNull();
        termination.Request.CreatedBy.Should().Be(FromIdentity);
        termination.Request.CreatedByDevice.Should().Be(FromDevice);

        termination.Response.Should().BeNull();
    }

    [Fact]
    public void Cannot_Request_Termination_For_Pending_Relationships()
    {
        var relationship = CreatePendingRelationship();

        Action acting = () => relationship.RequestTermination(FromIdentity, FromDevice);
        acting.Should().Throw<DomainException>().WithError(DomainErrors.OnlyActiveRelationshipsCanBeTerminated());
    }

    #region Accept Termination

    [Fact]
    public void Accepting_TerminationRequest_Changes_Relevant_Data()
    {
        var relationship = CreateRelationshipWithOpenTermination();
        var change = relationship.Changes.GetOpenTermination();

        relationship.AcceptChange(change.Id, ToIdentity, ToDevice, null);

        relationship.Status.Should().Be(RelationshipStatus.Terminated);

        change.Status.Should().Be(RelationshipChangeStatus.Accepted);

        change.Response.Should().NotBeNull();
        change.Response!.CreatedBy.Should().Be(ToIdentity);
        change.Response!.CreatedByDevice.Should().Be(ToDevice);
    }

    [Fact]
    public void Cannot_Accept_Already_Completed_TerminationRequests()
    {
        var relationship = CreateRelationshipWithOpenTermination();
        var change = relationship.Changes.GetOpenTermination();

        relationship.AcceptChange(change.Id, ToIdentity, ToDevice, ResponseContent);

        Action acting = () => relationship.AcceptChange(change.Id, ToIdentity, ToDevice, ResponseContent);
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestIsAlreadyCompleted());
    }

    [Fact]
    public void TerminationRequest_Cannot_Be_Accepted_By_Creator()
    {
        var relationship = CreateRelationshipWithOpenTermination();
        var change = relationship.Changes.GetOpenTermination();

        Action acting = () => relationship.AcceptChange(change.Id, FromIdentity, FromDevice, null);
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestCannotBeAcceptedByCreator());
    }

    #endregion

    #region Reject Termination

    [Fact]
    public void Rejecting_TerminationRequest_Changes_Relevant_Data()
    {
        var relationship = CreateRelationshipWithOpenTermination();
        var change = relationship.Changes.GetOpenTermination();

        relationship.RejectChange(change.Id, ToIdentity, ToDevice, null);

        relationship.Status.Should().Be(RelationshipStatus.Active);

        change.Status.Should().Be(RelationshipChangeStatus.Rejected);

        change.Response.Should().NotBeNull();
        change.Response!.CreatedBy.Should().Be(ToIdentity);
        change.Response!.CreatedByDevice.Should().Be(ToDevice);
    }

    [Fact]
    public void Cannot_Reject_Already_Completed_TerminationRequests()
    {
        var relationship = CreateRelationshipWithOpenTermination();
        var change = relationship.Changes.GetOpenTermination();

        relationship.RejectChange(change.Id, ToIdentity, ToDevice, ResponseContent);

        Action acting = () => relationship.RejectChange(change.Id, ToIdentity, ToDevice, ResponseContent);
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestIsAlreadyCompleted());
    }

    [Fact]
    public void TerminationRequest_Cannot_Be_Rejected_By_Creator()
    {
        var relationship = CreateRelationshipWithOpenTermination();
        var change = relationship.Changes.GetOpenTermination();

        Action acting = () => relationship.RejectChange(change.Id, FromIdentity, FromDevice, null);
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestCannotBeRejectedByCreator());
    }

    #endregion

    #region Revoke Termination

    [Fact]
    public void Revoking_TerminationRequest_Sets_Relevant_Data()
    {
        var relationship = CreateRelationshipWithOpenTermination();
        var change = relationship.Changes.GetOpenTermination();

        relationship.RevokeChange(change.Id, FromIdentity, FromDevice, null);

        relationship.Status.Should().Be(RelationshipStatus.Active);

        change.Status.Should().Be(RelationshipChangeStatus.Revoked);

        change.Response.Should().NotBeNull();
        change.Response!.CreatedBy.Should().Be(FromIdentity);
        change.Response!.CreatedByDevice.Should().Be(FromDevice);
    }

    [Fact]
    public void Cannot_Revoke_Already_Completed_TerminationRequests()
    {
        var relationship = CreateRelationshipWithOpenTermination();
        var change = relationship.Changes.GetOpenTermination();

        relationship.RevokeChange(change.Id, FromIdentity, FromDevice, ResponseContent);

        Action acting = () => relationship.RevokeChange(change.Id, FromIdentity, FromDevice, ResponseContent);
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestIsAlreadyCompleted());
    }

    [Fact]
    public void TerminationRequest_Cannot_Be_Revoked_By_Recipient()
    {
        var relationship = CreateRelationshipWithOpenTermination();
        var change = relationship.Changes.GetOpenTermination();

        Action acting = () => relationship.RevokeChange(change.Id, ToIdentity, ToDevice, null);
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestCanOnlyBeRevokedByCreator());
    }

    #endregion

    #endregion

    #region Constructor Functions

    private static Relationship CreatePendingRelationship()
    {
        var relationship = new Relationship(Template, FromIdentity, FromDevice, RequestContent);
        return relationship;
    }

    private static Relationship CreateActiveRelationship()
    {
        var relationship = new Relationship(Template, FromIdentity, FromDevice, RequestContent);
        var change = relationship.Changes.GetOpenCreation();
        relationship.AcceptChange(change.Id, ToIdentity, ToDevice, ResponseContent);
        return relationship;
    }

    private static Relationship CreateRelationshipWithOpenTermination()
    {
        var relationship = new Relationship(Template, FromIdentity, FromDevice, RequestContent);
        var change = relationship.Changes.GetOpenCreation();
        relationship.AcceptChange(change.Id, ToIdentity, ToDevice, ResponseContent);

        relationship.RequestTermination(FromIdentity, FromDevice);
        return relationship;
    }

    #endregion
}
