using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Tooling;

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

    public IdentityStatus IdentityStatus { get; internal set; }
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

    public void StartDeletionProcess(DeviceId asDevice)
    {
        EnsureNoActiveProcessExists();
        _deletionProcesses.Add(IdentityDeletionProcess.Create(Address, asDevice));
    }

    public void StartDeletionProcess()
    {
        EnsureNoActiveProcessExists();
        _deletionProcesses.Add(IdentityDeletionProcess.Create(Address));
    }

    private void EnsureNoActiveProcessExists()
    {
        var activeProcessExists = DeletionProcesses.Any(d => d.IsActive());

        if (activeProcessExists)
            throw new DomainException(DomainErrors.OnlyOneActiveDeletionProcessAllowed());
    }

    public void MarkAsToBeDeleted()
    {
        if (IdentityStatus == IdentityStatus.Deleting)
        {
            throw new DomainException(DomainErrors.CannotChangeIdentityStatusForIdentityUndergoingDeletion());
        }

        if (DeletionProcesses.Any(dp => dp.IsApproved()) == false)
        {
            throw new DomainException(DomainErrors.CannotMarkIdentityAsToBeDeletedIfNoApprovedDeletionProcessExists());
        }

        IdentityStatus = IdentityStatus.ToBeDeleted;
    }
}
public enum IdentityStatus
{
    Active,
    ToBeDeleted,
    Deleting
}

public enum DeletionProcessStatus
{
    WaitingForApproval,
    Approved
}
