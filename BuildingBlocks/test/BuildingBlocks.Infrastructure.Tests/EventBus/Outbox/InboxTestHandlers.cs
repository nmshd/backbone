using Microsoft.EntityFrameworkCore;
#pragma warning disable IDE0130 // Exercise the production module-to-DbContext routing with test handlers.
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.Tests.EventBus.Outbox;

namespace Backbone.Modules.OutboxTests.Module;

public class TestHandler(OutboxTestDbContext context) : IDomainEventHandler<OutboxTestDomainEvent>
{
    public async Task Handle(OutboxTestDomainEvent domainEvent)
    {
        var entity = new TestEntity { Id = domainEvent.DomainEventId, Name = domainEvent.Name };
        entity.Raise("handled");
        context.Add(entity);
        await context.SaveChangesAsync();
    }
}

public class FailingHandler(OutboxTestDbContext context) : IDomainEventHandler<OutboxTestDomainEvent>
{
    public async Task Handle(OutboxTestDomainEvent domainEvent)
    {
        await new TestHandler(context).Handle(domainEvent);
        throw new InvalidOperationException("Simulated handler crash after SaveChanges.");
    }
}

public class IndexRace
{
    private int _calls;
    private readonly TaskCompletionSource _bothReady = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public Task Wait()
    {
        if (Interlocked.Increment(ref _calls) >= 2)
            _bothReady.TrySetResult();
        return _bothReady.Task;
    }
}

public class AllocatingIndexHandler(OutboxTestDbContext context, IndexRace race) : IDomainEventHandler<OutboxTestDomainEvent>
{
    public async Task Handle(OutboxTestDomainEvent domainEvent)
    {
        var index = await context.Set<TestEntity>().CountAsync();
        await race.Wait();
        var entity = new TestEntity { Id = index.ToString(), Name = "allocated" };
        entity.Raise("created");
        context.Add(entity);
        await context.SaveChangesAsync();
    }
}
