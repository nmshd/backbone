﻿using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class Device
{
#pragma warning disable CS8618
    private Device() { }
#pragma warning restore CS8618

#pragma warning disable CS8618
    public Device(Identity identity, DeviceId? createdByDevice = null)
#pragma warning restore CS8618
    {
        Id = DeviceId.New();
        CreatedAt = SystemTime.UtcNow;
        CreatedByDevice = createdByDevice ?? Id;

        // The following distinction is unfortunatly necessary in order to make EF recognize that the identity already exists
        if (identity.IsNew())
            Identity = identity;
        else
            IdentityAddress = identity.Address;
    }

    public DeviceId Id { get; set; }

    public IdentityAddress IdentityAddress { get; set; }
    public Identity Identity { get; set; }

    public ApplicationUser User { get; set; }

    public DateTime CreatedAt { get; set; }

    public DeviceId CreatedByDevice { get; set; }

    public DateTime? DeletedAt { get; set; }
    public DeviceId? DeletedByDevice { get; set; }

    public bool IsOnboarded => User.HasLoggedIn;

    public static Expression<Func<Device, bool>> IsNotDeleted =>
        device => device.DeletedAt == null && device.DeletedByDevice == null;

    private DomainError? CanBeDeletedBy(IdentityAddress addressOfActiveIdentity)
    {
        if (IsOnboarded)
            return new DomainError("error.platform.validation.device.deviceCannotBeDeleted", "The device cannot be deleted because it is already onboarded.");

        if (Identity.Address != addressOfActiveIdentity)
            return new DomainError("error.platform.validation.device.deviceCannotBeDeleted", "You are not allowed to delete this device as it belongs to another identity.");

        return null;
    }

    public void MarkAsDeleted(DeviceId deletedByDevice, IdentityAddress addressOfActiveIdentity)
    {
        var error = CanBeDeletedBy(addressOfActiveIdentity);

        if (error != null)
            throw new DomainException(error);

        DeletedAt = SystemTime.UtcNow;
        DeletedByDevice = deletedByDevice;
    }
}
