using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;
public class IdentityTests
{
    [Fact]
    public void CannotMarkIdentityAsToBeDeletedIfNoDeletionProcessesExist()
    {
        // Arrange
        var identity = GenerateIdentity();
        identity.DeletionProcesses.Should().BeEmpty();

        // Act
        var acting = identity.MarkAsToBeDeleted;

        // Assert
        acting.Should().Throw<DomainException>()
            .WithMessage(DomainErrors.CannotMarkIdentityAsToBeDeletedIfNoApprovedDeletionProcessExists().Message);
    }

    [Fact]
    public void CannotMarkIdentityAsToBeDeletedIfNoDeletionProcessesAreApproved()
    {
        // Arrange
        var identity = GenerateIdentity();
        identity.StartDeletionProcess(DeviceId.New());

        // Act
        var acting = identity.MarkAsToBeDeleted;

        // Assert
        acting.Should().Throw<DomainException>()
            .WithMessage(DomainErrors.CannotMarkIdentityAsToBeDeletedIfNoApprovedDeletionProcessExists().Message);
    }

    [Fact]
    public void CannotMarkIdentityAsToBeDeletedIfIsAlreadyBeingDeleted()
    {
        // Arrange
        var identity = GenerateIdentity();
        identity.IdentityStatus = IdentityStatus.Deleting;

        // Act
        var acting = identity.MarkAsToBeDeleted;

        // Assert
        acting.Should().Throw<DomainException>()
            .WithMessage(DomainErrors.CannotChangeIdentityStatusForIdentityUndergoingDeletion().Message);
    }

    [Fact]
    public void IdentityWithApprovedDeletionProcessCanBeMarkedAsToBeDeleted()
    {
        // Arrange
        var identity = GenerateIdentity();
        identity.StartDeletionProcess(DeviceId.New());
        identity.DeletionProcesses.First().Status = DeletionProcessStatus.Approved;

        // Act
        identity.MarkAsToBeDeleted();

        // Assert
        identity.IdentityStatus.Should().Be(IdentityStatus.ToBeDeleted);
    }

    private Identity GenerateIdentity()
    {
        var identityAddress = IdentityAddress.Create(Array.Empty<byte>(), "id1");
        return new Identity("", identityAddress, Array.Empty<byte>(), TierId.Generate(), 1);
    }
}
