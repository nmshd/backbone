﻿using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.BuildingBlocks.Domain.Errors;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class Identity
{
    private readonly List<IdentityDeletionProcess> _deletionProcesses;


    public Identity(string? clientId, IdentityAddress address, byte[] publicKey, TierId tierId, byte identityVersion)
    {
        ClientId = clientId;
        Address = address;
        PublicKey = publicKey;
        IdentityVersion = identityVersion;
        CreatedAt = SystemTime.UtcNow;
        Devices = new List<Device>();
        TierId = tierId;
        _deletionProcesses = new List<IdentityDeletionProcess>();
    }

    public string? ClientId { get; private set; }

    public IdentityAddress Address { get; private set; }
    public byte[] PublicKey { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public List<Device> Devices { get; }

    public byte IdentityVersion { get; private set; }

    public TierId? TierId { get; private set; }

    public IReadOnlyList<IdentityDeletionProcess> DeletionProcesses => _deletionProcesses;

    public bool IsNew()
    {
        return Devices.Count < 1;
    }

    public void ChangeTier(TierId id)
    {
        if (TierId == id)
        {
            throw new DomainException(GenericDomainErrors.NewAndOldParametersMatch("TierId"));
        }

        TierId = id;
    }

    public void StartDeletionProcess(DeviceId asDevice, IHasher hasher)
    {
        EnsureNoActiveProcessExists();
        _deletionProcesses.Add(IdentityDeletionProcess.Create(Address, asDevice, hasher));
    }

    public void StartDeletionProcess(IHasher hasher)
    {
        EnsureNoActiveProcessExists();
        _deletionProcesses.Add(IdentityDeletionProcess.Create(Address, hasher));
    }

    private void EnsureNoActiveProcessExists()
    {
        var activeProcessExists = DeletionProcesses.Any(d => d.IsActive());

        if (activeProcessExists)
            throw new DomainException(DomainErrors.OnlyOneActiveDeletionProcessAllowed());
    }
}

public enum DeletionProcessStatus
{
    WaitingForApproval
}
