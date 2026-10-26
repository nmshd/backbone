using System.Collections.Concurrent;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.BuildingBlocks.Infrastructure.EventBus.Outbox;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Backbone.BuildingBlocks.Infrastructure.Tests.EventBus.Outbox;

// These tests create and remove only uniquely named databases of their own.
// Set BACKBONE_OUTBOX_TEST_POSTGRES / BACKBONE_OUTBOX_TEST_SQLSERVER to an administrative test connection.
public class OutboxPublisherTests : AbstractTestsBase
{
    [Theory]
    [InlineData("POSTGRES")]
    [InlineData("SQLSERVER")]
    public async Task Broker_failure_is_durable_and_a_new_worker_retries_the_same_event(string provider)
    {
        await using var test = await TestDatabase.Create(provider);
        await test.Seed("one");
        A.CallTo(() => test.Bus.Publish(A<DomainEvent>._)).ThrowsAsync(new IOException("broker unavailable"));
        await test.Publisher().PublishBatch(test.Database, TestContext.Current.CancellationToken);
        await using (var scope = test.Services.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<OutboxTestDbContext>();
            var row = await context.Set<OutboxMessage>().SingleAsync(TestContext.Current.CancellationToken);
            row.Attempts.ShouldBe(1);
            row.PublishedAt.ShouldBeNull();
            row.LeaseId.ShouldBeNull();
            row.LastError.ShouldBe(nameof(IOException));
            row.NextAttemptAt.ShouldBeGreaterThan(row.CreatedAt);
            row.NextAttemptAt = DateTime.UtcNow.AddSeconds(-1);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }
        A.CallTo(() => test.Bus.Publish(A<DomainEvent>._)).Returns(Task.CompletedTask);
        await test.Publisher().PublishBatch(test.Database, TestContext.Current.CancellationToken);
        var published = await test.Rows();
        published.Single().PublishedAt.ShouldNotBeNull();
        published.Single().Attempts.ShouldBe(2);
    }

    [Theory]
    [InlineData("POSTGRES")]
    [InlineData("SQLSERVER")]
    public async Task Concurrent_workers_claim_disjoint_batches_and_publish_every_event(string provider)
    {
        await using var test = await TestDatabase.Create(provider);
        await test.Seed(Enumerable.Range(0, 24).Select(x => "entity-" + x).ToArray());
        var claims = await Task.WhenAll(test.Store.Claim(test.Database, 12, Guid.NewGuid(), 30, TestContext.Current.CancellationToken),
            test.Store.Claim(test.Database, 12, Guid.NewGuid(), 30, TestContext.Current.CancellationToken));
        claims.SelectMany(x => x).Select(x => x.Id).Distinct().Count().ShouldBe(24);
        await test.ExpireLeases();
        var published = new ConcurrentBag<string>();
        A.CallTo(() => test.Bus.Publish(A<DomainEvent>._)).Invokes((DomainEvent e) => published.Add(e.DomainEventId)).Returns(Task.CompletedTask);
        await Task.WhenAll(test.Publisher().PublishBatch(test.Database, TestContext.Current.CancellationToken), test.Publisher().PublishBatch(test.Database, TestContext.Current.CancellationToken));
        published.Count.ShouldBe(24);
        published.Distinct().Count().ShouldBe(24);
        (await test.Rows()).ShouldAllBe(x => x.PublishedAt != null);
    }

    [Theory]
    [InlineData("POSTGRES")]
    [InlineData("SQLSERVER")]
    public async Task Crash_after_broker_acknowledgement_redelivers_with_the_original_event_id(string provider)
    {
        await using var test = await TestDatabase.Create(provider);
        await test.Seed("one");
        var oldLease = Guid.NewGuid();
        var message = (await test.Store.Claim(test.Database, 1, oldLease, 30, TestContext.Current.CancellationToken)).Single();
        // The broker accepted the event, but the worker died before calling Complete.
        var acceptedIds = new List<string> { message.EventId };
        await test.ExpireLeases();
        A.CallTo(() => test.Bus.Publish(A<DomainEvent>._)).Invokes((DomainEvent e) => acceptedIds.Add(e.DomainEventId)).Returns(Task.CompletedTask);
        await test.Publisher().PublishBatch(test.Database, TestContext.Current.CancellationToken);
        acceptedIds.Count.ShouldBe(2);
        acceptedIds.Distinct().Count().ShouldBe(1);
        (await test.Store.Complete(test.Database, message.Id, oldLease, TestContext.Current.CancellationToken)).ShouldBe(0);
        (await test.Rows()).Single().PublishedAt.ShouldNotBeNull();
    }

    [Theory]
    [InlineData("POSTGRES")]
    [InlineData("SQLSERVER")]
    public async Task Earlier_stream_events_block_successors_and_poison_events_are_isolated(string provider)
    {
        await using var test = await TestDatabase.Create(provider);
        await using (var scope = test.Services.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<OutboxTestDbContext>();
            var entity = new TestEntity { Id = "same-stream", Name = "test" };
            entity.Raise("first");
            entity.Raise("second");
            context.Add(entity);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }
        A.CallTo(() => test.Bus.Publish(A<DomainEvent>._)).ThrowsAsync(new IOException("unavailable"));
        await test.Publisher(new OutboxOptions { MaxAttempts = 1 }).PublishBatch(test.Database, TestContext.Current.CancellationToken);
        var rows = await test.Rows();
        rows.Count(x => x.FailedAt != null).ShouldBe(1);
        rows.Count(x => x.Attempts == 0).ShouldBe(1);
        (await test.Store.Claim(test.Database, 10, Guid.NewGuid(), 30, TestContext.Current.CancellationToken)).ShouldBeEmpty();
    }

    [Theory]
    [InlineData("POSTGRES")]
    [InlineData("SQLSERVER")]
    public async Task Lease_is_renewed_while_a_slow_broker_call_is_in_progress(string provider)
    {
        await using var test = await TestDatabase.Create(provider);
        await test.Seed("one");
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        A.CallTo(() => test.Bus.Publish(A<DomainEvent>._)).ReturnsLazily(() =>
        {
            started.SetResult();
            return release.Task;
        });
        var publishing = test.Publisher(new OutboxOptions { LeaseSeconds = 1 }).PublishBatch(test.Database, TestContext.Current.CancellationToken);
        try
        {
            await started.Task.WaitAsync(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken);
            await Task.Delay(1500, TestContext.Current.CancellationToken);
            (await test.Store.Claim(test.Database, 1, Guid.NewGuid(), 30, TestContext.Current.CancellationToken)).ShouldBeEmpty();
        }
        finally
        {
            release.TrySetResult();
            await publishing;
        }
        (await test.Rows()).Single().PublishedAt.ShouldNotBeNull();
    }

    [Theory]
    [InlineData("POSTGRES")]
    [InlineData("SQLSERVER")]
    public async Task Inbox_receipt_and_handler_changes_commit_atomically_and_suppress_duplicates(string provider)
    {
        await using var test = await TestDatabase.Create(provider);
        var domainEvent = new OutboxTestDomainEvent { Name = "inbox-test" };
        await InboxDispatcher.Handle(test.Services, typeof(Backbone.Modules.OutboxTests.Module.TestHandler), domainEvent);
        await InboxDispatcher.Handle(test.Services, typeof(Backbone.Modules.OutboxTests.Module.TestHandler), domainEvent);
        await using var scope = test.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<OutboxTestDbContext>();
        (await context.Set<TestEntity>().CountAsync(TestContext.Current.CancellationToken)).ShouldBe(1);
        (await context.Set<InboxMessage>().CountAsync(TestContext.Current.CancellationToken)).ShouldBe(1);
        (await context.Set<OutboxMessage>().CountAsync(TestContext.Current.CancellationToken)).ShouldBe(1);
    }

    [Theory]
    [InlineData("POSTGRES")]
    [InlineData("SQLSERVER")]
    public async Task Failed_handler_rolls_back_inbox_business_data_and_outgoing_events(string provider)
    {
        await using var test = await TestDatabase.Create(provider);
        await Should.ThrowAsync<InvalidOperationException>(() => InboxDispatcher.Handle(test.Services,
            typeof(Backbone.Modules.OutboxTests.Module.FailingHandler), new OutboxTestDomainEvent()));
        await using var scope = test.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<OutboxTestDbContext>();
        (await context.Set<TestEntity>().CountAsync(TestContext.Current.CancellationToken)).ShouldBe(0);
        (await context.Set<InboxMessage>().CountAsync(TestContext.Current.CancellationToken)).ShouldBe(0);
        (await context.Set<OutboxMessage>().CountAsync(TestContext.Current.CancellationToken)).ShouldBe(0);
    }

    [Theory]
    [InlineData("POSTGRES")]
    [InlineData("SQLSERVER")]
    public async Task Same_save_event_order_does_not_depend_on_the_insert_order(string provider)
    {
        await using var test = await TestDatabase.Create(provider);
        await using (var scope = test.Services.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<OutboxTestDbContext>();
            context.AddOutboxEvent(new OutboxTestDomainEvent { Name = "first" }, "stream");
            context.AddOutboxEvent(new OutboxTestDomainEvent { Name = "second" }, "stream");
            var messages = context.ChangeTracker.Entries<OutboxMessage>().ToArray();
            messages[0].Entity.Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
            messages[1].Entity.Id = Guid.Parse("00000000-0000-0000-0000-000000000001");
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }
        var names = new List<string>();
        A.CallTo(() => test.Bus.Publish(A<DomainEvent>._)).Invokes((DomainEvent e) => names.Add(((OutboxTestDomainEvent)e).Name)).Returns(Task.CompletedTask);
        await test.Publisher().PublishBatch(test.Database, TestContext.Current.CancellationToken);
        names.ShouldBe(["first"]);
        await test.Publisher().PublishBatch(test.Database, TestContext.Current.CancellationToken);
        names.ShouldBe(["first", "second"]);
    }

    [Theory]
    [InlineData("POSTGRES")]
    [InlineData("SQLSERVER")]
    public async Task Concurrent_index_allocation_retries_the_entire_inbox_handler_in_a_fresh_scope(string provider)
    {
        await using var test = await TestDatabase.Create(provider);
        await Task.WhenAll(
            InboxDispatcher.Handle(test.Services, typeof(Backbone.Modules.OutboxTests.Module.AllocatingIndexHandler), new OutboxTestDomainEvent()),
            InboxDispatcher.Handle(test.Services, typeof(Backbone.Modules.OutboxTests.Module.AllocatingIndexHandler), new OutboxTestDomainEvent()));
        await using var scope = test.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<OutboxTestDbContext>();
        (await context.Set<TestEntity>().CountAsync(TestContext.Current.CancellationToken)).ShouldBe(2);
        (await context.Set<InboxMessage>().CountAsync(TestContext.Current.CancellationToken)).ShouldBe(2);
        (await context.Set<OutboxMessage>().CountAsync(TestContext.Current.CancellationToken)).ShouldBe(2);
    }

    [Theory]
    [InlineData("POSTGRES")]
    [InlineData("SQLSERVER")]
    public async Task Explicit_rollback_restores_database_generated_outbox_values_for_retry(string provider)
    {
        await using var test = await TestDatabase.Create(provider);
        await using var scope = test.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<OutboxTestDbContext>();
        var entity = new TestEntity { Id = "one", Name = "test" };
        entity.Raise("event");
        context.Add(entity);
        await context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(TestContext.Current.CancellationToken);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
            await transaction.RollbackAsync(TestContext.Current.CancellationToken);
        });
        entity.DomainEvents.Count.ShouldBe(1);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var rows = await test.Rows();
        rows.Count.ShouldBe(1);
        rows[0].Sequence.ShouldBeGreaterThan(0);
        entity.DomainEvents.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("POSTGRES")]
    [InlineData("SQLSERVER")]
    public async Task Publisher_restores_correlation_and_trace_context(string provider)
    {
        await using var test = await TestDatabase.Create(provider);
        string traceId;
        using (var activity = new System.Diagnostics.Activity("request").SetIdFormat(System.Diagnostics.ActivityIdFormat.W3C).Start())
        using (Backbone.BuildingBlocks.Infrastructure.CorrelationIds.CustomLogContext.SetCorrelationId("request-correlation"))
        {
            traceId = activity.TraceId.ToString();
            await test.Seed("one");
        }
        var actualCorrelation = "";
        var actualTraceId = "";
        A.CallTo(() => test.Bus.Publish(A<DomainEvent>._)).Invokes(() =>
        {
            actualCorrelation = Backbone.BuildingBlocks.Infrastructure.CorrelationIds.CustomLogContext.GetCorrelationId();
            actualTraceId = System.Diagnostics.Activity.Current!.TraceId.ToString();
        }).Returns(Task.CompletedTask);
        using (Backbone.BuildingBlocks.Infrastructure.CorrelationIds.CustomLogContext.SetCorrelationId("worker-context"))
            await test.Publisher().PublishBatch(test.Database, TestContext.Current.CancellationToken);
        actualCorrelation.ShouldBe("request-correlation");
        actualTraceId.ShouldBe(traceId);
    }

}

internal sealed class TestDatabase : IAsyncDisposable
{
    public ServiceProvider Services { get; }
    public IEventBus Bus { get; } = A.Fake<IEventBus>();
    public OutboxDatabase Database { get; } = new(typeof(OutboxTestDbContext), "OutboxTests");
    public OutboxStore Store => Services.GetRequiredService<OutboxStore>();

    private TestDatabase(string provider, string connection)
    {
        var services = new ServiceCollection();
        services.AddSingleton(Bus);
        services.AddSingleton(Database);
        services.AddSingleton<OutboxStore>();
        services.AddSingleton<OutboxDiagnostics>();
        services.AddTransient<Backbone.Modules.OutboxTests.Module.TestHandler>();
        services.AddTransient<Backbone.Modules.OutboxTests.Module.FailingHandler>();
        services.AddTransient<Backbone.Modules.OutboxTests.Module.AllocatingIndexHandler>();
        services.AddSingleton<Backbone.Modules.OutboxTests.Module.IndexRace>();
        services.AddDbContext<OutboxTestDbContext>(builder =>
        {
            if (provider == "POSTGRES") builder.UseNpgsql(connection, x => x.EnableRetryOnFailure());
            else builder.UseSqlServer(connection, x => x.EnableRetryOnFailure());
        });
        Services = services.BuildServiceProvider();
    }

    public static async Task<TestDatabase> Create(string provider)
    {
        var connection = Environment.GetEnvironmentVariable("BACKBONE_OUTBOX_TEST_" + provider);
        Assert.SkipWhen(string.IsNullOrWhiteSpace(connection), "No dedicated " + provider + " test database connection configured.");
        var name = "backbone_outbox_" + Guid.NewGuid().ToString("N");
        if (provider == "POSTGRES") connection = new Npgsql.NpgsqlConnectionStringBuilder(connection) { Database = name }.ConnectionString;
        else connection = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connection) { InitialCatalog = name }.ConnectionString;
        var result = new TestDatabase(provider, connection!);
        await using var scope = result.Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<OutboxTestDbContext>().Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);
        return result;
    }

    public OutboxPublisher Publisher(OutboxOptions? options = null) => new([Database], Store, Bus, Options.Create(options ?? new OutboxOptions()),
        Services.GetRequiredService<OutboxDiagnostics>(), NullLogger<OutboxPublisher>.Instance);

    public async Task Seed(params string[] ids)
    {
        await using var scope = Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<OutboxTestDbContext>();
        foreach (var id in ids)
        {
            var entity = new TestEntity { Id = id, Name = "test" };
            entity.Raise(id);
            context.Add(entity);
        }
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public async Task<List<OutboxMessage>> Rows()
    {
        await using var scope = Services.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<OutboxTestDbContext>().Set<OutboxMessage>().AsNoTracking().ToListAsync(TestContext.Current.CancellationToken);
    }

    public async Task ExpireLeases()
    {
        await using var scope = Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<OutboxTestDbContext>().Set<OutboxMessage>()
            .ExecuteUpdateAsync(x => x.SetProperty(m => m.LeaseExpiresAt, DateTime.UtcNow.AddSeconds(-1)), TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await using (var scope = Services.CreateAsyncScope())
            await scope.ServiceProvider.GetRequiredService<OutboxTestDbContext>().Database.EnsureDeletedAsync(CancellationToken.None);
        await Services.DisposeAsync();
    }
}
