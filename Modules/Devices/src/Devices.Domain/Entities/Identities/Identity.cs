using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;
using Entity = Backbone.BuildingBlocks.Domain.Entity;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class Identity : Entity
{
    private readonly List<IdentityDeletionProcess> _deletionProcesses;
    protected virtual EfCoreFeatureFlagSet EfCoreFeatureFlagSetDoNotUse { get; } = [];
    private TierId? _tierId;

    // ReSharper disable once UnusedMember.Local
    protected Identity()
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

    public virtual List<Device> Devices { get; }

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

    public virtual IReadOnlyList<IdentityDeletionProcess> DeletionProcesses => _deletionProcesses;

    public DateTime? DeletionGracePeriodEndsAt { get; private set; }

    public IdentityStatus Status { get; private set; }

    public bool IsGracePeriodOver => DeletionGracePeriodEndsAt != null && DeletionGracePeriodEndsAt < SystemTime.UtcNow;
    public virtual FeatureFlagSet FeatureFlags => EfCoreFeatureFlagSetDoNotUse;

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

    public IdentityDeletionProcess StartDeletionProcess(DeviceId asDevice, double? lengthOfGracePeriodInDays = null)
    {
        EnsureNoActiveProcessExists();
        EnsureIdentityOwnsDevice(asDevice);

        TierIdBeforeDeletion = TierId;

        var deletionProcess = IdentityDeletionProcess.Start(Address, asDevice, lengthOfGracePeriodInDays);
        _deletionProcesses.Add(deletionProcess);

        DeletionGracePeriodEndsAt = deletionProcess.GracePeriodEndsAt;
        TierId = Tier.QUEUED_FOR_DELETION.Id;
        Status = IdentityStatus.ToBeDeleted;
        RaiseDomainEvent(new IdentityToBeDeletedDomainEvent(Address, deletionProcess.GracePeriodEndsAt!.Value));

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

    public IdentityDeletionProcess CancelDeletionProcess(IdentityDeletionProcessId deletionProcessId, DeviceId cancelledByDeviceId)
    {
        EnsureIdentityOwnsDevice(cancelledByDeviceId);

        var deletionProcess = GetDeletionProcessWithId(deletionProcessId);
        deletionProcess.EnsureStatus(DeletionProcessStatus.Approved);

        deletionProcess.Cancel(Address, cancelledByDeviceId);
        TierId = TierIdBeforeDeletion ?? throw new Exception($"Error when trying to cancel deletion process: '{nameof(TierIdBeforeDeletion)}' is null.");
        TierIdBeforeDeletion = null;
        DeletionGracePeriodEndsAt = null;
        Status = IdentityStatus.Active;

        RaiseDomainEvent(new IdentityDeletionCancelledDomainEvent(Address));

        return deletionProcess;
    }

    public void HandleErrorDuringDeletion(string errorMessage)
    {
        var deletionProcess = DeletionProcesses.SingleOrDefault(dp => dp.Status == DeletionProcessStatus.Deleting)
                              ?? throw new DomainException(DomainErrors.DeletionProcessMustBeInStatus(DeletionProcessStatus.Deleting));
        deletionProcess.ErrorDuringDeletion(Address, errorMessage);
    }

    private IdentityDeletionProcess GetDeletionProcessWithId(IdentityDeletionProcessId deletionProcessId)
    {
        return DeletionProcesses.FirstOrDefault(x => x.Id == deletionProcessId) ?? throw new DomainException(GenericDomainErrors.NotFound(nameof(IdentityDeletionProcess)));
    }

    public static Identity CreateTestIdentity(IdentityAddress address, byte[] publicKey, TierId tierId, string username)
    {
        return new Identity("test", address, publicKey, tierId, 1, CommunicationLanguage.DEFAULT_LANGUAGE, username);
    }

    public void ChangeFeatureFlags(Dictionary<FeatureFlagName, bool> featureFlags)
    {
        foreach (var keyValuePair in featureFlags)
        {
            FeatureFlags.Set(keyValuePair.Key, keyValuePair.Value);
        }

        RaiseDomainEvent(new FeatureFlagsOfIdentityChangedDomainEvent(this));
    }

    #region Expressions

    public static Expression<Func<Identity, bool>> HasAddress(IdentityAddress address)
    {
        return i => i.Address == address.ToString();
    }

    public static Expression<Func<Identity, bool>> HasAddress(List<IdentityAddress> recipients)
    {
        return i => recipients.Contains(i.Address);
    }

    public static Expression<Func<Identity, bool>> IsReadyForDeletion()
    {
        return i => i.Status == IdentityStatus.ToBeDeleted && i.DeletionGracePeriodEndsAt != null && i.DeletionGracePeriodEndsAt < SystemTime.UtcNow;
    }

    public static Expression<Func<Identity, bool>> HasUser(string username)
    {
        return i => i.Devices.Any(d => d.User.UserName == username);
    }

    public static Expression<Func<Identity, bool>> HasDeletionProcessInStatus(DeletionProcessStatus requiredStatus)
    {
        return i => i.DeletionProcesses.Any(p => p.Status == requiredStatus);
    }

    #endregion
}

public enum DeletionProcessStatus
{
    WaitingForApproval = 0,
    Approved = 1,
    Cancelled = 2,
    Deleting = 10
}

public enum IdentityStatus
{
    Active = 0,
    ToBeDeleted = 1,
    Deleting = 2
}
