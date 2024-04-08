using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Errors;
using Backbone.Modules.Relationships.Domain.Ids;
using Backbone.Modules.Relationships.Domain.Tests.Extensions;
using Backbone.Tooling;
using Backbone.UnitTestTools.Data;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Relationships.Domain.Tests.Tests;

public class RelationshipTests
{
    private static readonly IdentityAddress FROM_IDENTITY = IdentityAddress.Create([1, 1, 1], "prod.enmeshed.eu");
    private static readonly DeviceId FROM_DEVICE = DeviceId.New();

    private static readonly IdentityAddress TO_IDENTITY = IdentityAddress.Create([2, 2, 2], "prod.enmeshed.eu");
    private static readonly DeviceId TO_DEVICE = DeviceId.New();

    private static readonly byte[] REQUEST_CONTENT = [1, 1, 1];
    private static readonly byte[] RESPONSE_CONTENT = [2, 2, 2];

    private static readonly RelationshipTemplate TEMPLATE = new(TO_IDENTITY, TO_DEVICE, 1, SystemTime.UtcNow.AddDays(1), [0]);

    #region Creation

    [Fact]
    public void New_Relationship_Has_Correct_Data()
    {
        // Act
        var relationship = CreatePendingRelationship();

        // Assert
        relationship.From.Should().Be(FROM_IDENTITY);
        relationship.To.Should().Be(TO_IDENTITY);
        relationship.Status.Should().Be(RelationshipStatus.Pending);

        relationship.Changes.Should().HaveCount(1);
        var change = relationship.Changes.GetLatestOfType(RelationshipChangeType.Creation);
        change.Request.CreatedBy.Should().Be(FROM_IDENTITY);
        change.Request.CreatedByDevice.Should().Be(FROM_DEVICE);

        change.Type.Should().Be(RelationshipChangeType.Creation);
        change.Status.Should().Be(RelationshipChangeStatus.Pending);

        change.Response.Should().BeNull();
    }

    #region Accept Creation

    [Fact]
    public void Accepting_CreationRequest_Changes_Relevant_Data()
    {
        // Arrange
        var relationship = CreatePendingRelationship();
        var change = relationship.Changes.GetOpenCreation()!;

        // Act
        relationship.AcceptChange(change.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);

        // Assert
        relationship.Status.Should().Be(RelationshipStatus.Active);

        change.Status.Should().Be(RelationshipChangeStatus.Accepted);

        change.Response.Should().NotBeNull();
        change.Response!.Content.Should().Equal(RESPONSE_CONTENT);
        change.Response!.CreatedBy.Should().Be(TO_IDENTITY);
        change.Response!.CreatedByDevice.Should().Be(TO_DEVICE);
    }

    [Fact]
    public void Cannot_Accept_CreationRequests_Without_Content()
    {
        // Arrange
        var relationship = CreatePendingRelationship();
        var change = relationship.Changes.GetOpenCreation()!;

        // Act
        Action acting = () => relationship.AcceptChange(change.Id, TO_IDENTITY, TO_DEVICE, null);

        // Assert
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ContentIsRequiredForCompletingRelationships());
    }

    [Fact]
    public void Cannot_Accept_Already_Completed_CreationRequests()
    {
        // Arrange
        var relationship = CreatePendingRelationship();
        var change = relationship.Changes.GetOpenCreation()!;
        relationship.AcceptChange(change.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);

        // Act
        Action acting = () => relationship.AcceptChange(change.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);

        // Assert
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestIsAlreadyCompleted());
    }

    [Fact]
    public void Relationship_Cannot_Be_Accepted_By_Creator()
    {
        // Arrange
        var relationship = CreatePendingRelationship();
        var change = relationship.Changes.GetOpenCreation()!;

        // Act
        Action acting = () => relationship.AcceptChange(change.Id, FROM_IDENTITY, FROM_DEVICE, RESPONSE_CONTENT);

        // Assert
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestCannotBeAcceptedByCreator());
    }

    #endregion

    #region Reject Creation

    [Fact]
    public void Rejecting_CreationRequest_Sets_Relevant_Data()
    {
        // Arrange
        var relationship = CreatePendingRelationship();
        var change = relationship.Changes.GetOpenCreation()!;

        // Act
        relationship.RejectChange(change.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);

        // Assert
        relationship.Status.Should().Be(RelationshipStatus.Rejected);

        change.Status.Should().Be(RelationshipChangeStatus.Rejected);

        change.Response.Should().NotBeNull();
        change.Response!.Content.Should().Equal(RESPONSE_CONTENT);
        change.Response!.CreatedBy.Should().Be(TO_IDENTITY);
        change.Response!.CreatedByDevice.Should().Be(TO_DEVICE);
    }

    [Fact]
    public void Cannot_Reject_CreationRequests_Without_Content()
    {
        // Arrange
        var relationship = CreatePendingRelationship();
        var change = relationship.Changes.GetOpenCreation()!;

        // Act
        Action acting = () => relationship.RejectChange(change.Id, TO_IDENTITY, TO_DEVICE, null);

        // Assert
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ContentIsRequiredForCompletingRelationships());
    }

    [Fact]
    public void Cannot_Reject_Already_Completed_CreationRequests()
    {
        // Arrange
        var relationship = CreatePendingRelationship();
        var change = relationship.Changes.GetOpenCreation()!;
        relationship.RejectChange(change.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);

        // Act
        Action acting = () => relationship.RejectChange(change.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);

        // Assert
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestIsAlreadyCompleted());
    }

    [Fact]
    public void CreationRequest_Cannot_Be_Rejected_By_Creator()
    {
        // Arrange
        var relationship = CreatePendingRelationship();
        var change = relationship.Changes.GetOpenCreation()!;

        // Act
        Action acting = () => relationship.RejectChange(change.Id, FROM_IDENTITY, FROM_DEVICE, RESPONSE_CONTENT);

        // Assert
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestCannotBeRejectedByCreator());
    }

    #endregion

    #region Revoke Creation

    [Fact]
    public void Revoking_CreationRequest_Sets_Relevant_Data()
    {
        // Arrange
        var relationship = CreatePendingRelationship();
        var change = relationship.Changes.GetOpenCreation()!;

        // Act
        relationship.RevokeChange(change.Id, FROM_IDENTITY, FROM_DEVICE, RESPONSE_CONTENT);

        // Assert
        relationship.Status.Should().Be(RelationshipStatus.Revoked);

        change.Status.Should().Be(RelationshipChangeStatus.Revoked);

        change.Response.Should().NotBeNull();
        change.Response!.Content.Should().Equal(RESPONSE_CONTENT);
        change.Response!.CreatedBy.Should().Be(FROM_IDENTITY);
        change.Response!.CreatedByDevice.Should().Be(FROM_DEVICE);
    }

    [Fact]
    public void Cannot_Revoke_CreationRequests_Without_Content()
    {
        // Arrange
        var relationship = CreatePendingRelationship();
        var change = relationship.Changes.GetOpenCreation()!;

        // Act
        Action acting = () => relationship.RevokeChange(change.Id, FROM_IDENTITY, FROM_DEVICE, null);

        // Assert
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ContentIsRequiredForCompletingRelationships());
    }

    [Fact]
    public void Cannot_Revoke_Already_Completed_CreationRequests()
    {
        // Arrange
        var relationship = CreatePendingRelationship();
        var change = relationship.Changes.GetOpenCreation()!;
        relationship.RevokeChange(change.Id, FROM_IDENTITY, FROM_DEVICE, RESPONSE_CONTENT);

        // Act
        Action acting = () => relationship.RevokeChange(change.Id, FROM_IDENTITY, FROM_DEVICE, RESPONSE_CONTENT);

        // Assert
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestIsAlreadyCompleted());
    }

    [Fact]
    public void CreationRequest_Cannot_Be_Revoked_By_Recipient()
    {
        // Arrange
        var relationship = CreatePendingRelationship();
        var change = relationship.Changes.GetOpenCreation()!;

        // Act
        Action acting = () => relationship.RevokeChange(change.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);

        // Assert
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestCanOnlyBeRevokedByCreator());
    }

    #endregion

    #endregion

    #region Common

    [Fact]
    public void Cannot_Accept_Non_Existent_ChangeRequests()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        Action acting = () => relationship.AcceptChange(RelationshipChangeId.New(), TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);

        // Assert
        acting.Should().Throw<DomainException>().WithError(GenericDomainErrors.NotFound());
    }

    [Fact]
    public void Cannot_Reject_Non_Existent_ChangeRequests()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        Action acting = () => relationship.RejectChange(RelationshipChangeId.New(), TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);

        // Assert
        acting.Should().Throw<DomainException>().WithError(GenericDomainErrors.NotFound());
    }

    [Fact]
    public void Cannot_Revoke_Non_Existent_ChangeRequests()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        Action acting = () => relationship.RevokeChange(RelationshipChangeId.New(), TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);

        // Assert
        acting.Should().Throw<DomainException>().WithError(GenericDomainErrors.NotFound());
    }

    #endregion

    #region Termination

    [Fact]
    public void Requesting_Termination_Sets_Relevant_Data()
    {
        // Arrange
        var relationship = CreateActiveRelationship();

        // Act
        relationship.RequestTermination(FROM_IDENTITY, FROM_DEVICE);

        // Assert
        relationship.Changes.Should().HaveCount(2);

        var termination = relationship.Changes.GetOpenTermination()!;
        termination.Should().NotBeNull();
        termination.Status.Should().Be(RelationshipChangeStatus.Pending);
        termination.Request.Should().NotBeNull();
        termination.Request.CreatedBy.Should().Be(FROM_IDENTITY);
        termination.Request.CreatedByDevice.Should().Be(FROM_DEVICE);

        termination.Response.Should().BeNull();
    }

    [Fact]
    public void Cannot_Request_Termination_For_Pending_Relationships()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        Action acting = () => relationship.RequestTermination(FROM_IDENTITY, FROM_DEVICE);

        // Assert
        acting.Should().Throw<DomainException>().WithError(DomainErrors.OnlyActiveRelationshipsCanBeTerminated());
    }

    #region Accept Termination

    [Fact]
    public void Accepting_TerminationRequest_Changes_Relevant_Data()
    {
        // Arrange
        var relationship = CreateRelationshipWithOpenTermination();
        var change = relationship.Changes.GetOpenTermination()!;

        // Act
        relationship.AcceptChange(change.Id, TO_IDENTITY, TO_DEVICE, null);

        // Assert
        relationship.Status.Should().Be(RelationshipStatus.Terminated);

        change.Status.Should().Be(RelationshipChangeStatus.Accepted);

        change.Response.Should().NotBeNull();
        change.Response!.CreatedBy.Should().Be(TO_IDENTITY);
        change.Response!.CreatedByDevice.Should().Be(TO_DEVICE);
    }

    [Fact]
    public void Cannot_Accept_Already_Completed_TerminationRequests()
    {
        // Arrange
        var relationship = CreateRelationshipWithOpenTermination();
        var change = relationship.Changes.GetOpenTermination()!;

        // Act
        relationship.AcceptChange(change.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);

        Action acting = () => relationship.AcceptChange(change.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);

        // Assert
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestIsAlreadyCompleted());
    }

    [Fact]
    public void TerminationRequest_Cannot_Be_Accepted_By_Creator()
    {
        // Arrange
        var relationship = CreateRelationshipWithOpenTermination();
        var change = relationship.Changes.GetOpenTermination()!;

        // Act
        Action acting = () => relationship.AcceptChange(change.Id, FROM_IDENTITY, FROM_DEVICE, null);

        // Assert
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestCannotBeAcceptedByCreator());
    }

    #endregion

    #region Reject Termination

    [Fact]
    public void Rejecting_TerminationRequest_Changes_Relevant_Data()
    {
        // Arrange
        var relationship = CreateRelationshipWithOpenTermination();
        var change = relationship.Changes.GetOpenTermination()!;

        // Act
        relationship.RejectChange(change.Id, TO_IDENTITY, TO_DEVICE, null);

        // Assert
        relationship.Status.Should().Be(RelationshipStatus.Active);

        change.Status.Should().Be(RelationshipChangeStatus.Rejected);

        change.Response.Should().NotBeNull();
        change.Response!.CreatedBy.Should().Be(TO_IDENTITY);
        change.Response!.CreatedByDevice.Should().Be(TO_DEVICE);
    }

    [Fact]
    public void Cannot_Reject_Already_Completed_TerminationRequests()
    {
        // Arrange
        var relationship = CreateRelationshipWithOpenTermination();
        var change = relationship.Changes.GetOpenTermination()!;

        // Act
        relationship.RejectChange(change.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);

        Action acting = () => relationship.RejectChange(change.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);

        // Assert
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestIsAlreadyCompleted());
    }

    [Fact]
    public void TerminationRequest_Cannot_Be_Rejected_By_Creator()
    {
        // Arrange
        var relationship = CreateRelationshipWithOpenTermination();
        var change = relationship.Changes.GetOpenTermination()!;

        // Act
        Action acting = () => relationship.RejectChange(change.Id, FROM_IDENTITY, FROM_DEVICE, null);

        // Assert
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestCannotBeRejectedByCreator());
    }

    #endregion

    #region Revoke Termination

    [Fact]
    public void Revoking_TerminationRequest_Sets_Relevant_Data()
    {
        // Arrange
        var relationship = CreateRelationshipWithOpenTermination();
        var change = relationship.Changes.GetOpenTermination()!;

        // Act
        relationship.RevokeChange(change.Id, FROM_IDENTITY, FROM_DEVICE, null);

        // Assert
        relationship.Status.Should().Be(RelationshipStatus.Active);

        change.Status.Should().Be(RelationshipChangeStatus.Revoked);

        change.Response.Should().NotBeNull();
        change.Response!.CreatedBy.Should().Be(FROM_IDENTITY);
        change.Response!.CreatedByDevice.Should().Be(FROM_DEVICE);
    }

    [Fact]
    public void Cannot_Revoke_Already_Completed_TerminationRequests()
    {
        // Arrange
        var relationship = CreateRelationshipWithOpenTermination();
        var change = relationship.Changes.GetOpenTermination()!;

        relationship.RevokeChange(change.Id, FROM_IDENTITY, FROM_DEVICE, RESPONSE_CONTENT);

        // Act
        Action acting = () => relationship.RevokeChange(change.Id, FROM_IDENTITY, FROM_DEVICE, RESPONSE_CONTENT);

        // Assert
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestIsAlreadyCompleted());
    }

    [Fact]
    public void TerminationRequest_Cannot_Be_Revoked_By_Recipient()
    {
        // Arrange
        var relationship = CreateRelationshipWithOpenTermination();
        var change = relationship.Changes.GetOpenTermination()!;

        // Act
        Action acting = () => relationship.RevokeChange(change.Id, TO_IDENTITY, TO_DEVICE, null);

        // Assert
        acting.Should().Throw<DomainException>().WithError(DomainErrors.ChangeRequestCanOnlyBeRevokedByCreator());
    }

    #endregion

    #endregion

    #region Selectors

    [Fact]
    public void WithParticipant_From()
    {
        // Arrange
        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var relationship = CreateActiveRelationship(identityAddress);

        // Act
        var result = relationship.HasParticipant(identityAddress);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void WithParticipant_To()
    {
        // Arrange
        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var relationship = CreateActiveRelationship((null, identityAddress));

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
        var identityAddressFrom = TestDataGenerator.CreateRandomIdentityAddress();
        var identityAddressTo = TestDataGenerator.CreateRandomIdentityAddress();
        var relationship = CreateActiveRelationship((identityAddressFrom, identityAddressTo));

        // Act
        var hasIdentityAddressFrom = relationship.HasParticipant(identityAddressFrom);
        var hasIdentityAddressTo = relationship.HasParticipant(identityAddressTo);

        // Assert
        hasIdentityAddressFrom.Should().BeTrue();
        hasIdentityAddressTo.Should().BeTrue();
        relationship.From.Should().Be(identityAddressFrom);
    }

    #endregion

    #region Constructor Functions

    private static Relationship CreatePendingRelationship()
    {
        var relationship = new Relationship(TEMPLATE, FROM_IDENTITY, FROM_DEVICE, REQUEST_CONTENT);
        return relationship;
    }

    private static Relationship CreateActiveRelationship()
    {
        var relationship = new Relationship(TEMPLATE, FROM_IDENTITY, FROM_DEVICE, REQUEST_CONTENT);
        var change = relationship.Changes.GetOpenCreation();
        relationship.AcceptChange(change!.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);
        return relationship;
    }

    private static Relationship CreateActiveRelationship(IdentityAddress from)
    {
        return CreateActiveRelationship((from, null));
    }

    private static Relationship CreateActiveRelationship((IdentityAddress? from, IdentityAddress? to) parameters)
    {
        RelationshipTemplate? template = null;
        if (parameters.to is not null)
        {
            template = new RelationshipTemplate(parameters.to, TO_DEVICE, 1, SystemTime.UtcNow.AddDays(1), [0]);
        }

        var relationship = new Relationship(template ?? TEMPLATE, parameters.from ?? FROM_IDENTITY, FROM_DEVICE, REQUEST_CONTENT);
        var change = relationship.Changes.GetOpenCreation()!;
        relationship.AcceptChange(change.Id, parameters.to ?? TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);
        return relationship;
    }

    private static Relationship CreateRelationshipWithOpenTermination()
    {
        var relationship = new Relationship(TEMPLATE, FROM_IDENTITY, FROM_DEVICE, REQUEST_CONTENT);
        var change = relationship.Changes.GetOpenCreation();
        relationship.AcceptChange(change!.Id, TO_IDENTITY, TO_DEVICE, RESPONSE_CONTENT);

        relationship.RequestTermination(FROM_IDENTITY, FROM_DEVICE);
        return relationship;
    }

    #endregion
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
