using System.Linq.Expressions;
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

    public IdentityStatus Status { get; private set; }

    public string? ClientId { get; private set; }

    public IdentityAddress Address { get; }
    public byte[] PublicKey { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public List<Device> Devices { get; }

    public byte IdentityVersion { get; private set; }

    public TierId? TierId { get; private set; }

    public IReadOnlyList<IdentityDeletionProcess> DeletionProcesses => _deletionProcesses;

    public DateTime? DeletionGracePeriodEndsAt { get; internal set; }

    public bool IsNew()
    {
        return Devices.Count < 1;
    }

    public void ChangeTier(TierId id)
    {
        if (id == Tier.QUEUED_FOR_DELETION.Id || TierId == Tier.QUEUED_FOR_DELETION.Id)
            throw new DomainException(DomainErrors.CannotChangeTierQueuedForDeletion());

        if (TierId == id)
            throw new DomainException(GenericDomainErrors.NewAndOldParametersMatch("TierId"));

        TierId = id;
    }

    public IdentityDeletionProcess StartDeletionProcessAsSupport()
    {
        EnsureNoActiveProcessExists();

        var deletionProcess = IdentityDeletionProcess.StartAsSupport(Address);
        _deletionProcesses.Add(deletionProcess);

        return deletionProcess;
    }

    public IdentityDeletionProcess StartDeletionProcessAsOwner(DeviceId asDevice)
    {
        EnsureNoActiveProcessExists();

        var deletionProcess = IdentityDeletionProcess.StartAsOwner(Address, asDevice);
        _deletionProcesses.Add(deletionProcess);

        DeletionGracePeriodEndsAt = deletionProcess.GracePeriodEndsAt;

        return deletionProcess;
    }

    public void DeletionStarted()
    {
        var deletionProcess = DeletionProcesses.SingleOrDefault(dp => dp.IsActive())
            ?? throw new DomainException(DomainErrors.NoActiveDeletionProcessFound());
        Status = IdentityStatus.Deleting;
        deletionProcess.DeletionStarted();
    }

    public void DeletionProcessApprovalReminder1Sent()
    {
        EnsureDeletionProcessInStatusExists(DeletionProcessStatus.WaitingForApproval);

        var deletionProcess = GetDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval)!;
        deletionProcess.ApprovalReminder1Sent(Address);
    }

    public void DeletionProcessApprovalReminder2Sent()
    {
        EnsureDeletionProcessInStatusExists(DeletionProcessStatus.WaitingForApproval);

        var deletionProcess = GetDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval)!;
        deletionProcess.ApprovalReminder2Sent(Address);
    }

    public void DeletionProcessApprovalReminder3Sent()
    {
        EnsureDeletionProcessInStatusExists(DeletionProcessStatus.WaitingForApproval);

        var deletionProcess = GetDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval)!;
        deletionProcess.ApprovalReminder3Sent(Address);
    }

    private void EnsureDeletionProcessInStatusExists(DeletionProcessStatus status)
    {
        var deletionProcess = DeletionProcesses.Any(d => d.Status == status);

        if (!deletionProcess)
            throw new DomainException(DomainErrors.NoDeletionProcessWithRequiredStatusExists());
    }

    private void EnsureNoActiveProcessExists()
    {
        var activeProcessExists = DeletionProcesses.Any(d => d.IsActive());

        if (activeProcessExists)
            throw new DomainException(DomainErrors.OnlyOneActiveDeletionProcessAllowed());
    }

    public void DeletionGracePeriodReminder1Sent()
    {
        EnsureDeletionProcessInStatusExists(DeletionProcessStatus.Approved);

        var deletionProcess = GetDeletionProcessInStatus(DeletionProcessStatus.Approved)!;
        deletionProcess.GracePeriodReminder1Sent(Address);
    }

    public void DeletionGracePeriodReminder2Sent()
    {
        EnsureDeletionProcessInStatusExists(DeletionProcessStatus.Approved);

        var deletionProcess = DeletionProcesses.First(d => d.Status == DeletionProcessStatus.Approved);
        deletionProcess.GracePeriodReminder2Sent(Address);
    }

    public void DeletionGracePeriodReminder3Sent()
    {
        EnsureDeletionProcessInStatusExists(DeletionProcessStatus.Approved);

        var deletionProcess = DeletionProcesses.First(d => d.Status == DeletionProcessStatus.Approved);
        deletionProcess.GracePeriodReminder3Sent(Address);
    }

    public IdentityDeletionProcess? GetDeletionProcessInStatus(DeletionProcessStatus deletionProcessStatus)
    {
        return DeletionProcesses.FirstOrDefault(x => x.Status == deletionProcessStatus);
    }

    public static Expression<Func<Identity, bool>> IsWithAddress(IdentityAddress address)
    {
        return i => i.Address == address;
    }
}

public enum IdentityStatus
{
    Active = 0,
    ToBeDeleted = 1,
    Deleting = 2
}

public enum DeletionProcessStatus
{
    WaitingForApproval = 0,
    Approved = 1,
    Deleting = 2
}
