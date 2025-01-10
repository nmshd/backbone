using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;
using CSharpFunctionalExtensions;
using Entity = Backbone.BuildingBlocks.Domain.Entity;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class Identity : Entity
{
    private readonly List<IdentityDeletionProcess> _deletionProcesses;
    private TierId? _tierId;

    // ReSharper disable once UnusedMember.Local
    private Identity()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        ClientId = null!;
        Address = null!;
        PublicKey = null!;
        Devices = null!;
        _deletionProcesses = null!;
    }

    private Identity(string clientId, IdentityAddress address, byte[] publicKey, TierId tierId, byte identityVersion, CommunicationLanguage deviceCommunicationLanguage, string username)
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

        Devices.Add(Device.CreateTestDevice(this, deviceCommunicationLanguage, username));

        RaiseDomainEvent(new IdentityCreatedDomainEvent(this));
    }

    public Identity(string clientId, IdentityAddress address, byte[] publicKey, TierId tierId, byte identityVersion, CommunicationLanguage deviceCommunicationLanguage)
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

        Devices.Add(new Device(this, deviceCommunicationLanguage));

        RaiseDomainEvent(new IdentityCreatedDomainEvent(this));
    }

    public string? ClientId { get; }

    public IdentityAddress Address { get; }
    public byte[] PublicKey { get; }
    public DateTime CreatedAt { get; }

    public List<Device> Devices { get; }

    public byte IdentityVersion { get; private set; }

    public TierId? TierIdBeforeDeletion { get; private set; }

    public TierId TierId
    {
        get => _tierId!; // the only time the backing field is null is within the constructor, so we can suppress the warning
        private set
        {
            if (value == _tierId) return;

            var oldTier = _tierId;

            _tierId = value;

            // if the oldTier was null, we don't consider it a change
            if (oldTier != null)
                RaiseDomainEvent(new TierOfIdentityChangedDomainEvent(this, oldTier, value));
        }
    }

    public IReadOnlyList<IdentityDeletionProcess> DeletionProcesses => _deletionProcesses;

    public DateTime? DeletionGracePeriodEndsAt { get; private set; }

    public IdentityStatus Status { get; private set; }

    public bool IsGracePeriodOver => DeletionGracePeriodEndsAt != null && DeletionGracePeriodEndsAt < SystemTime.UtcNow;

    public bool IsNew()
    {
        return Devices.Count < 1;
    }

    public Device AddDevice(CommunicationLanguage communicationLanguage, DeviceId createdByDevice, bool isBackupDevice)
    {
        var newDevice = new Device(this, communicationLanguage, createdByDevice, isBackupDevice);
        Devices.Add(newDevice);
        return newDevice;
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

    public IdentityDeletionProcess StartDeletionProcessAsOwner(DeviceId asDevice, double? lengthOfGracePeriodInDays = null)
    {
        EnsureNoActiveProcessExists();
        EnsureIdentityOwnsDevice(asDevice);

        TierIdBeforeDeletion = TierId;

        var deletionProcess = IdentityDeletionProcess.StartAsOwner(Address, asDevice, lengthOfGracePeriodInDays);
        _deletionProcesses.Add(deletionProcess);

        DeletionGracePeriodEndsAt = deletionProcess.GracePeriodEndsAt;
        TierId = Tier.QUEUED_FOR_DELETION.Id;
        Status = IdentityStatus.ToBeDeleted;
        RaiseDomainEvent(new IdentityToBeDeletedDomainEvent(Address, deletionProcess.GracePeriodEndsAt!.Value));

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

    public Result<IdentityDeletionProcess, DomainError> CancelStaleDeletionProcess()
    {
        var deletionProcess = GetDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval);

        if (deletionProcess == null)
            return Result.Failure<IdentityDeletionProcess, DomainError>(DomainErrors.DeletionProcessMustBeInStatus(DeletionProcessStatus.WaitingForApproval));
        if (!deletionProcess.HasApprovalPeriodExpired)
            return Result.Failure<IdentityDeletionProcess, DomainError>(DomainErrors.DeletionProcessMustBePastDueApproval());

        deletionProcess.Cancel(Address);

        return Result.Success<IdentityDeletionProcess, DomainError>(deletionProcess);
    }

    public IdentityDeletionProcess ApproveDeletionProcess(IdentityDeletionProcessId deletionProcessId, DeviceId deviceId)
    {
        EnsureIdentityOwnsDevice(deviceId);

        TierIdBeforeDeletion = TierId;

        var deletionProcess = GetDeletionProcess(deletionProcessId);
        deletionProcess.Approve(Address, deviceId);

        Status = IdentityStatus.ToBeDeleted;
        RaiseDomainEvent(new IdentityToBeDeletedDomainEvent(Address, deletionProcess.GracePeriodEndsAt!.Value));
        DeletionGracePeriodEndsAt = deletionProcess.GracePeriodEndsAt;
        TierId = Tier.QUEUED_FOR_DELETION.Id;

        return deletionProcess;
    }

    public void DeletionStarted()
    {
        var deletionProcess = DeletionProcesses.SingleOrDefault(dp => dp.Status == DeletionProcessStatus.Approved)
                              ?? throw new DomainException(DomainErrors.DeletionProcessMustBeInStatus(DeletionProcessStatus.Approved));

        deletionProcess.DeletionStarted(Address);
        Status = IdentityStatus.Deleting;
        RaiseDomainEvent(new IdentityDeletedDomainEvent(Address));
    }

    private IdentityDeletionProcess GetDeletionProcess(IdentityDeletionProcessId deletionProcessId)
    {
        var deletionProcess = DeletionProcesses.FirstOrDefault(x => x.Id == deletionProcessId) ?? throw new DomainException(GenericDomainErrors.NotFound(nameof(IdentityDeletionProcess)));
        return deletionProcess;
    }

    public IdentityDeletionProcess RejectDeletionProcess(IdentityDeletionProcessId deletionProcessId, DeviceId deviceId)
    {
        EnsureIdentityOwnsDevice(deviceId);

        var deletionProcess = GetDeletionProcess(deletionProcessId);
        deletionProcess.Reject(Address, deviceId);

        return deletionProcess;
    }

    private void EnsureDeletionProcessInStatusExists(DeletionProcessStatus status)
    {
        var deletionProcess = DeletionProcesses.Any(d => d.Status == status);

        if (!deletionProcess)
            throw new DomainException(DomainErrors.DeletionProcessMustBeInStatus(status));
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

    public IdentityDeletionProcess CancelDeletionProcessAsOwner(IdentityDeletionProcessId deletionProcessId, DeviceId cancelledByDeviceId)
    {
        EnsureIdentityOwnsDevice(cancelledByDeviceId);

        var deletionProcess = GetDeletionProcessWithId(deletionProcessId);
        deletionProcess.EnsureStatus(DeletionProcessStatus.Approved);

        deletionProcess.CancelAsOwner(Address, cancelledByDeviceId);
        TierId = TierIdBeforeDeletion ?? throw new Exception($"Error when trying to cancel deletion process: '{nameof(TierIdBeforeDeletion)}' is null.");
        TierIdBeforeDeletion = null;
        Status = IdentityStatus.Active;

        RaiseDomainEvent(new IdentityDeletionCancelledDomainEvent(Address));

        return deletionProcess;
    }

    public IdentityDeletionProcess CancelDeletionProcessAsSupport(IdentityDeletionProcessId deletionProcessId)
    {
        var deletionProcess = GetDeletionProcessWithId(deletionProcessId);
        deletionProcess.EnsureStatus(DeletionProcessStatus.Approved);

        deletionProcess.CancelAsSupport(Address);
        TierId = TierIdBeforeDeletion ?? throw new Exception($"Error when trying to cancel deletion process: '{nameof(TierIdBeforeDeletion)}' is null.");
        TierIdBeforeDeletion = null;
        Status = IdentityStatus.Active;

        RaiseDomainEvent(new IdentityDeletionCancelledDomainEvent(Address));

        return deletionProcess;
    }

    private IdentityDeletionProcess GetDeletionProcessWithId(IdentityDeletionProcessId deletionProcessId)
    {
        return DeletionProcesses.FirstOrDefault(x => x.Id == deletionProcessId) ?? throw new DomainException(GenericDomainErrors.NotFound(nameof(IdentityDeletionProcess)));
    }

    public static Identity CreateTestIdentity(IdentityAddress address, byte[] publicKey, TierId tierId, string username)
    {
        return new Identity("test", address, publicKey, tierId, 1, CommunicationLanguage.DEFAULT_LANGUAGE, username);
    }

    #region Expressions

    public static Expression<Func<Identity, bool>> HasAddress(IdentityAddress address)
    {
        return i => i.Address == address.ToString();
    }

    public static Expression<Func<Identity, bool>> ContainsAddressValue(IEnumerable<string> recipients)
    {
        return i => recipients.Contains(i.Address.Value);
    }

    public static Expression<Func<Identity, bool>> IsReadyForDeletion()
    {
        return i => i.Status == IdentityStatus.ToBeDeleted && i.DeletionGracePeriodEndsAt != null && i.DeletionGracePeriodEndsAt < SystemTime.UtcNow;
    }

    public static Expression<Func<Identity, bool>> HasUser(string username)
    {
        return i => i.Devices.Any(d => d.User.UserName == username);
    }

    #endregion
}

public enum DeletionProcessStatus
{
    WaitingForApproval = 0,
    Approved = 1,
    Cancelled = 2,
    Rejected = 3,
    Deleting = 10
}

public enum IdentityStatus
{
    Active = 0,
    ToBeDeleted = 1,
    Deleting = 2
}
