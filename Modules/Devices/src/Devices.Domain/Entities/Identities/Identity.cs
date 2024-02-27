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
        Devices = [];
        TierId = tierId;
        Status = IdentityStatus.Active;
        _deletionProcesses = [];
    }

    public string? ClientId { get; }

    public IdentityAddress Address { get; }
    public byte[] PublicKey { get; }
    public DateTime CreatedAt { get; }

    public List<Device> Devices { get; }

    public byte IdentityVersion { get; private set; }

    public TierId? TierIdBeforeDeletion { get; private set; }
    public TierId? TierId { get; private set; }

    public IReadOnlyList<IdentityDeletionProcess> DeletionProcesses => _deletionProcesses;

    public DateTime? DeletionGracePeriodEndsAt { get; private set; }

    public IdentityStatus Status { get; private set; }

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

        TierIdBeforeDeletion = TierId;

        var deletionProcess = IdentityDeletionProcess.StartAsOwner(Address, asDevice);
        _deletionProcesses.Add(deletionProcess);

        DeletionGracePeriodEndsAt = deletionProcess.GracePeriodEndsAt;
        TierId = Tier.QUEUED_FOR_DELETION.Id;
        Status = IdentityStatus.ToBeDeleted;

        return deletionProcess;
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

    public IdentityDeletionProcess ApproveDeletionProcess(IdentityDeletionProcessId deletionProcessId, DeviceId deviceId)
    {
        var deletionProcess = DeletionProcesses.FirstOrDefault(x => x.Id == deletionProcessId) ?? throw new DomainException(GenericDomainErrors.NotFound(nameof(IdentityDeletionProcess)));

        deletionProcess.Approve(Address, deviceId);

        Status = IdentityStatus.ToBeDeleted;
        DeletionGracePeriodEndsAt = deletionProcess.GracePeriodEndsAt;
        TierId = Tier.QUEUED_FOR_DELETION.Id;

        return deletionProcess;
    }

    public IdentityDeletionProcess RejectDeletionProcess(IdentityDeletionProcessId deletionProcessId, DeviceId deviceId)
    {
        EnsureIdentityOwnsDevice(deviceId);

        var deletionProcess = DeletionProcesses.FirstOrDefault(x => x.Id == deletionProcessId) ?? throw new DomainException(GenericDomainErrors.NotFound(nameof(IdentityDeletionProcess)));
        deletionProcess.Reject(Address, deviceId);

        return deletionProcess;
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

    private void EnsureIdentityOwnsDevice(DeviceId currentDeviceId)
    {
        if (!Devices.Exists(device => device.Id == currentDeviceId))
            throw new DomainException(GenericDomainErrors.NotFound(nameof(Device)));
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
}

public enum DeletionProcessStatus
{
    WaitingForApproval = 0,
    Approved = 1,
    Rejected = 3
}

public enum IdentityStatus
{
    Active = 0,
    ToBeDeleted = 1
}
