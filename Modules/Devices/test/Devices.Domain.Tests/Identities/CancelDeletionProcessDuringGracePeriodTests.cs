using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Domain.Tests.Identities.TestDoubles;
using Backbone.Tooling;
using FakeItEasy.Sdk;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;
public class CancelDeletionProcessDuringGracePeriodTests
{
    [Fact]
    public void Cancel_deletion_process_during_grace_period()
    {
        var currentDate = SystemTime.UtcNow.Date;
        SystemTime.Set(currentDate);

        var activeIdentity = CreateIdentity();
        var activeDevice = DeviceId.Parse("DVC");
        var deletionProcess = activeIdentity.StartDeletionProcessAsOwner(activeDevice);

        SystemTime.Set(currentDate.AddDays(5));

        // Act
        activeIdentity.CancelDeletionProcessDuringGracePeriod(deletionProcess);

        // Assert
        activeIdentity.DeletionProcesses.Should().HaveCount(0);
    }

    [Fact]
    public void Cancel_deletion_process_after_grace_period()
    {
        var currentDate = SystemTime.UtcNow.Date;
        SystemTime.Set(currentDate);

        var activeIdentity = CreateIdentity();
        var activeDevice = DeviceId.Parse("DVC");
        var deletionProcess = activeIdentity.StartDeletionProcessAsOwner(activeDevice);

        SystemTime.Set(currentDate.AddDays(35));

        // Act
        activeIdentity.CancelDeletionProcessDuringGracePeriod(deletionProcess);

        // Assert
        activeIdentity.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.deletionProcessGracePeriodHasEnded");

    }

    // TODO: write a negative test for 1) when deletion process doesn't exist, 2) when grace period has ended


    //[Fact]
    //public void Cancel_deletion_process_when_deletion_process_does_not_exists()
    //{
    //    // Arrange
    //    var currentDate = SystemTime.UtcNow.Date;
    //    SystemTime.Set(currentDate);

    //    var activeIdentity = CreateIdentity();
    //    var activeDevice = DeviceId.Parse("DVC");

    //    // Act
    //    activeIdentity.CancelDeletionProcessDuringGracePeriod(deletionProcess);

    //    // Assert
    //}


    private static Identity CreateIdentity()
    {
        var address = IdentityAddress.Create(Array.Empty<byte>(), "id1");
        return new Identity("", address, Array.Empty<byte>(), TierId.Generate(), 1);
    }
}
