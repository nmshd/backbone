using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;

namespace Backbone.Modules.Synchronization.Application.Tests;

public class SyncRunBuilder
{
    private static int _currentIndex;
    private const ushort DURATION = 10;
    private IdentityAddress _createdBy;
    private DeviceId _createdByDevice;
    private DateTime? _expiresAt;
    private IEnumerable<ExternalEvent> _externalEvents = new List<ExternalEvent>();
    private DateTime? _finalizedAt;
    private bool _isCanceled;
    private bool _isRunning;

    public SyncRunBuilder()
    {
        _createdBy = TestDataGenerator.CreateRandomIdentityAddress();
        _createdByDevice = TestDataGenerator.CreateRandomDeviceId();
        _isRunning = true;
    }

    public static SyncRunBuilder Build()
    {
        return new SyncRunBuilder();
    }

    public SyncRunBuilder CreatedBy(IdentityAddress createdBy)
    {
        _createdBy = createdBy;
        return this;
    }

    public SyncRunBuilder CreatedByDevice(DeviceId createdByDevice)
    {
        _createdByDevice = createdByDevice;
        return this;
    }

    public SyncRunBuilder Running()
    {
        _isRunning = true;
        return this;
    }

    public SyncRunBuilder Finalized()
    {
        _isRunning = false;
        return this;
    }

    public SyncRunBuilder FinalizedAt(DateTime finalizedAt)
    {
        _finalizedAt = finalizedAt;
        return this;
    }

    public SyncRunBuilder ExpiresAt(DateTime expiresAt)
    {
        _expiresAt = expiresAt;
        return this;
    }

    public SyncRunBuilder Canceled()
    {
        _isCanceled = true;
        return this;
    }

    public SyncRunBuilder WithExternalEvents(IEnumerable<ExternalEvent> externalEvents)
    {
        _externalEvents = externalEvents;
        return this;
    }

    public SyncRun Create()
    {
        var syncRun = new SyncRun(_currentIndex++, DURATION, _createdBy, _createdByDevice, _externalEvents, SyncRun.SyncRunType.ExternalEventSync);

        if (!_isRunning)
            syncRun.FinalizeExternalEventSync(Array.Empty<ExternalEventResult>());

        if (_expiresAt != null)
            syncRun.ExpiresAt = _expiresAt.Value;

        if (_isCanceled)
            syncRun.Cancel();

        if (_finalizedAt != null)
            syncRun.FinalizedAt = _finalizedAt.Value;

        return syncRun;
    }
}
