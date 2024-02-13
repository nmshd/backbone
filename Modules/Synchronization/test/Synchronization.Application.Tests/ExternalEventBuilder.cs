using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;

namespace Backbone.Modules.Synchronization.Application.Tests;

public class ExternalEventBuilder
{
    private static int _currentIndex;
    private byte _errorCount;
    private IdentityAddress _owner;
    private object _payload;
    private SyncRun? _syncRun;
    private ExternalEventType _type;

    private ExternalEventBuilder()
    {
        _type = ExternalEventType.MessageDelivered;
        _owner = TestDataGenerator.CreateRandomIdentityAddress();
        _payload = new { SomeArbitraryProperty = "SomeArbitraryValue" };
    }

    public static ExternalEventBuilder Build()
    {
        return new ExternalEventBuilder();
    }

    public ExternalEventBuilder WithType(ExternalEventType type)
    {
        _type = type;
        return this;
    }

    public ExternalEventBuilder WithPayload(object payload)
    {
        _payload = payload;
        return this;
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

    public ExternalEvent Create()
    {
        var externalEvent = new ExternalEvent(_type, _owner, _currentIndex++, _payload ?? new { someAribtraryProperty = "someArbitraryValue" })
        {
            SyncErrorCount = _errorCount
        };

        if (_syncRun != null)
            externalEvent.AssignToSyncRun(_syncRun);

        return externalEvent;
    }
}
