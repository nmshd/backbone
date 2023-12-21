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

    public string? ClientId { get; private set; }

    public IdentityAddress Address { get; private set; }
    public byte[] PublicKey { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public List<Device> Devices { get; }

    public byte IdentityVersion { get; private set; }

    public TierId? TierId { get; private set; }

    public IReadOnlyList<IdentityDeletionProcess> DeletionProcesses => _deletionProcesses;

    public DateTime? DeletionGracePeriodEndsAt { get; private set; }

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

    public void DeletionProcessApprovalReminder1Sent()
    {
        var deletionProcess = GetDeletionProcessWaitingForApproval();
        deletionProcess.ApprovalReminder1Sent(Address);
    }

    public void DeletionProcessApprovalReminder2Sent()
    {
        var deletionProcess = GetDeletionProcessWaitingForApproval();
        deletionProcess.ApprovalReminder2Sent(Address);
    }

    public void DeletionProcessApprovalReminder3Sent()
    {
        var deletionProcess = GetDeletionProcessWaitingForApproval();
        deletionProcess.ApprovalReminder3Sent(Address);
    }

    private IdentityDeletionProcess GetDeletionProcessWaitingForApproval()
    {
        return _deletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.WaitingForApproval) ?? throw new DomainException(GenericDomainErrors.NotFound(nameof(IdentityDeletionProcessId)));
    }

    private void EnsureNoActiveProcessExists()
    {
        var activeProcessExists = DeletionProcesses.Any(d => d.IsActive());

        if (activeProcessExists)
            throw new DomainException(DomainErrors.OnlyOneActiveDeletionProcessAllowed());
    }

    public void DeletionGracePeriodReminder1Sent()
    {
        EnsureApprovedProcessExists();

        var deletionProcess = GetDeletionProcessInStatus(DeletionProcessStatus.Approved)!;
        deletionProcess.GracePeriodReminder1Sent(Address);
    }

    public void DeletionGracePeriodReminder2Sent()
    {
        EnsureApprovedProcessExists();

        var deletionProcess = DeletionProcesses.First(d => d.Status == DeletionProcessStatus.Approved);
        deletionProcess.GracePeriodReminder2Sent(Address);
    }

    public void DeletionGracePeriodReminder3Sent()
    {
        EnsureApprovedProcessExists();

        var deletionProcess = DeletionProcesses.First(d => d.Status == DeletionProcessStatus.Approved);
        deletionProcess.GracePeriodReminder3Sent(Address);
    }

    private void EnsureApprovedProcessExists()
    {
        var approvedProcessExists = DeletionProcesses.Any(d => d.Status == DeletionProcessStatus.Approved);

        if (!approvedProcessExists)
            throw new DomainException(DomainErrors.NoApprovedDeletionProcessFound());
    }

    public IdentityDeletionProcess? GetDeletionProcessInStatus(DeletionProcessStatus deletionProcessStatus)
    {
        return DeletionProcesses.FirstOrDefault(x => x.Status == deletionProcessStatus);
    }
}

public enum DeletionProcessStatus
{
    WaitingForApproval = 0,
    Approved = 1
}
