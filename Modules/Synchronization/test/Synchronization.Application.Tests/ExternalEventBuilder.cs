using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;

namespace Backbone.Modules.Synchronization.Application.Tests;

public class ExternalEventBuilder
{
    private byte _errorCount;
    private IdentityAddress _owner;
    private SyncRun? _syncRun;
    private int _index;

    private ExternalEventBuilder()
    {
        _owner = TestDataGenerator.CreateRandomIdentityAddress();
    }

    public static ExternalEventBuilder Build()
    {
        return new ExternalEventBuilder();
    }

    public ExternalEventBuilder WithOwner(IdentityAddress owner)
    {
        _owner = owner;
        return this;
    }

    public ExternalEventBuilder WithMaxErrorCount()
    {
        return WithErrorCount(3);
    }

    public ExternalEventBuilder WithErrorCount(byte errorCount)
    {
        _errorCount = errorCount;
        return this;
    }

    public ExternalEventBuilder AssignedToSyncRun(SyncRun syncRun)
    {
        _syncRun = syncRun;
        return this;
    }

    public ExternalEventBuilder AlreadySynced()
    {
        _syncRun = SyncRunBuilder.Build().CreatedBy(_owner).Finalized().Create();
        return this;
    }

    public ExternalEventBuilder WithIndex(int index)
    {
        _index = index;
        return this;
    }

    public ExternalEvent Create()
    {
        var externalEvent = new MessageReceivedExternalEvent(_owner, new MessageReceivedExternalEvent.EventPayload { Id = "MSG11111111111111111" })
        {
            SyncErrorCount = _errorCount
        };

        externalEvent.UpdateIndex(_index);

        if (_syncRun != null)
            externalEvent.AssignToSyncRun(_syncRun);

        return externalEvent;
    }
}
