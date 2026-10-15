using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.BuildingBlocks.Infrastructure.EventBus.Outbox;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using FakeItEasy;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.BuildingBlocks.Infrastructure.Tests.EventBus.Outbox;

public class OutboxPersistenceTests : AbstractTestsBase
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task All_save_overloads_persist_events_without_calling_broker(bool asynchronous, bool explicitAccept)
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        var bus = A.Fake<IEventBus>();
        await using var context = Create(connection, bus);
        await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);
        var entity = new TestEntity { Id = "one", Name = "first" };
        entity.Raise("created");
        context.Add(entity);
        if (asynchronous)
        {
            if (explicitAccept) await context.SaveChangesAsync(true, TestContext.Current.CancellationToken);
            else await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }
        else
        {
            if (explicitAccept) context.SaveChanges(true);
            else context.SaveChanges();
        }
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        (await context.Set<TestEntity>().CountAsync(TestContext.Current.CancellationToken)).ShouldBe(1);
        var message = await context.Set<OutboxMessage>().SingleAsync(TestContext.Current.CancellationToken);
        message.EventId.ShouldBe(entity.LastEventId);
        message.EventName.ShouldBe("OutboxTest");
        message.Payload.ShouldContain("created");
        message.PublishedAt.ShouldBeNull();
        entity.DomainEvents.ShouldBeEmpty();
        A.CallTo(() => bus.Publish(A<DomainEvent>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Failed_save_is_atomic_and_can_be_retried_without_duplicate_events()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        await using var context = Create(connection);
        await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);
        var entity = new TestEntity { Id = "one", Name = null! };
        entity.Raise("created");
        context.Add(entity);
        await Should.ThrowAsync<DbUpdateException>(() => context.SaveChangesAsync(TestContext.Current.CancellationToken));
        await using (var verification = Create(connection))
        {
            (await verification.Set<TestEntity>().CountAsync(TestContext.Current.CancellationToken)).ShouldBe(0);
            (await verification.Set<OutboxMessage>().CountAsync(TestContext.Current.CancellationToken)).ShouldBe(0);
        }
        entity.DomainEvents.Count.ShouldBe(1);
        entity.Name = "fixed";
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        (await context.Set<OutboxMessage>().CountAsync(TestContext.Current.CancellationToken)).ShouldBe(1);
        entity.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    public async Task Failed_implicit_commit_retains_events_and_rolls_back_both_tables()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        var failure = new FailCommitInterceptor();
        var options = new DbContextOptionsBuilder<OutboxTestDbContext>().UseSqlite(connection).AddInterceptors(failure).Options;
        await using var context = new OutboxTestDbContext(options, A.Fake<IEventBus>());
        await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);
        var entity = new TestEntity { Id = "one", Name = "test" };
        entity.Raise("created");
        context.Add(entity);
        failure.FailNextCommit = true;
        await Should.ThrowAsync<IOException>(() => context.SaveChangesAsync(TestContext.Current.CancellationToken));
        entity.DomainEvents.Count.ShouldBe(1);
        await using (var verification = Create(connection))
        {
            (await verification.Set<TestEntity>().CountAsync(TestContext.Current.CancellationToken)).ShouldBe(0);
            (await verification.Set<OutboxMessage>().CountAsync(TestContext.Current.CancellationToken)).ShouldBe(0);
        }
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        (await context.Set<OutboxMessage>().CountAsync(TestContext.Current.CancellationToken)).ShouldBe(1);
        entity.DomainEvents.ShouldBeEmpty();
    }

    private sealed class FailCommitInterceptor : DbTransactionInterceptor
    {
        public bool FailNextCommit { get; set; }
        public override ValueTask<InterceptionResult> TransactionCommittingAsync(DbTransaction transaction, TransactionEventData eventData,
            InterceptionResult result, CancellationToken cancellationToken = default)
        {
            if (FailNextCommit)
            {
                FailNextCommit = false;
                throw new IOException("Simulated database commit failure.");
            }
            return ValueTask.FromResult(result);
        }
    }

    [Fact]
    public async Task Explicit_transaction_rolls_back_business_and_outbox_and_preserves_events_for_retry()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        await using var context = Create(connection);
        await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);
        var entity = new TestEntity { Id = "one", Name = "first" };
        entity.Raise("created");
        context.Add(entity);
        await using (var transaction = await context.Database.BeginTransactionAsync(TestContext.Current.CancellationToken))
        {
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
            entity.DomainEvents.Count.ShouldBe(1);
            await transaction.RollbackAsync(TestContext.Current.CancellationToken);
        }
        (await context.Set<OutboxMessage>().CountAsync(TestContext.Current.CancellationToken)).ShouldBe(0);
        (await context.Set<TestEntity>().CountAsync(TestContext.Current.CancellationToken)).ShouldBe(0);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        (await context.Set<OutboxMessage>().CountAsync(TestContext.Current.CancellationToken)).ShouldBe(1);
        entity.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    public async Task Commit_of_explicit_transaction_clears_events_once_and_supports_multiple_saves()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        await using var context = Create(connection);
        await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);
        var entity = new TestEntity { Id = "one", Name = "first" };
        entity.Raise("created");
        context.Add(entity);
        await using (var transaction = await context.Database.BeginTransactionAsync(TestContext.Current.CancellationToken))
        {
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
            entity.Raise("updated");
            entity.Name = "updated";
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
            await transaction.CommitAsync(TestContext.Current.CancellationToken);
        }
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        (await context.Set<OutboxMessage>().CountAsync(TestContext.Current.CancellationToken)).ShouldBe(2);
        entity.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    public async Task Save_without_accepting_changes_stages_only_one_outbox_message()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        await using var context = Create(connection);
        await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);
        var entity = new TestEntity { Id = "one", Name = "first" };
        entity.Raise("created");
        context.Add(entity);
        await context.SaveChangesAsync(false, TestContext.Current.CancellationToken);
        context.ChangeTracker.AcceptAllChanges();
        await context.SaveChangesAsync(false, TestContext.Current.CancellationToken);
        (await context.Set<OutboxMessage>().CountAsync(TestContext.Current.CancellationToken)).ShouldBe(1);
    }

    [Fact]
    public void Production_context_registration_does_not_resolve_the_broker()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IEventBus>(_ => throw new InvalidOperationException("Broker unavailable"));
        services.AddDbContextForModule<OutboxTestDbContext>(new DatabaseConfiguration
        {
            Provider = "Postgres", ConnectionString = "Host=localhost;Database=design;Username=design"
        }, "OutboxTests");
        using var container = services.BuildServiceProvider();
        using var scope = container.CreateScope();
        scope.ServiceProvider.GetRequiredService<OutboxTestDbContext>().ShouldNotBeNull();
    }

    [Fact]
    public async Task Disposing_an_uncommitted_transaction_preserves_events_for_a_later_save()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        await using var context = Create(connection);
        await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);
        var entity = new TestEntity { Id = "one", Name = "test" };
        entity.Raise("created");
        context.Add(entity);
        await using (var transaction = await context.Database.BeginTransactionAsync(TestContext.Current.CancellationToken))
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        entity.DomainEvents.Count.ShouldBe(1);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        (await context.Set<OutboxMessage>().CountAsync(TestContext.Current.CancellationToken)).ShouldBe(1);
        entity.DomainEvents.ShouldBeEmpty();
    }

    private static OutboxTestDbContext Create(SqliteConnection connection, IEventBus? bus = null) => new(
        new DbContextOptionsBuilder<OutboxTestDbContext>().UseSqlite(connection).Options, bus ?? A.Fake<IEventBus>());
}

public class OutboxTestDbContext(DbContextOptions<OutboxTestDbContext> options, IEventBus bus) : AbstractDbContextBase(options, bus)
{
    public OutboxTestDbContext(DbContextOptions<OutboxTestDbContext> options, IServiceProvider provider) : this(options, (IEventBus)null!)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("OutboxTests");
        builder.Entity<TestEntity>().HasKey(x => x.Id);
        builder.Entity<TestEntity>().Ignore(x => x.DomainEvents);
        builder.Entity<TestEntity>().Ignore(x => x.LastEventId);
        builder.Entity<TestEntity>().Property(x => x.Name).IsRequired();
    }
}

public class TestEntity : Entity
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string LastEventId { get; private set; } = "";
    public void Raise(string name)
    {
        var domainEvent = new OutboxTestDomainEvent { Name = name };
        LastEventId = domainEvent.DomainEventId;
        RaiseDomainEvent(domainEvent);
    }
}

public class OutboxTestDomainEvent : DomainEvent
{
    public string Name { get; set; } = "";
}
