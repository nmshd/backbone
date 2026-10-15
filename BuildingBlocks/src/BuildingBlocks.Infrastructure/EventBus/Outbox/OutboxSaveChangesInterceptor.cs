using System.Data.Common;
using System.Text.Json;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.Outbox;

// One instance per DbContext. Staged messages survive failed saves and explicit transaction rollbacks.
public sealed class OutboxSaveChangesInterceptor : SaveChangesInterceptor
{
    private bool _explicitTransaction;
    private readonly HashSet<DomainEvent> _persisted = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<DomainEvent, (Entity Entity, OutboxMessage Message)> _staged = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<object, SavedState> _saved = new(ReferenceEqualityComparer.Instance);

    private sealed record SavedState(EntityEntry Entry, EntityState State, PropertyValues Original, PropertyValues Current, HashSet<string> Temporary, HashSet<string> Modified);
    public DbTransactionInterceptor Transactions => new OutboxTransactionInterceptor(this);

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        Stage(eventData.Context!);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        Stage(eventData.Context!);
        return ValueTask.FromResult(result);
    }

    private void Stage(DbContext context)
    {
        if (context.Database.CurrentTransaction == null && _saved.Count != 0)
            Rollback(); // A caller disposed an explicit transaction without committing.
        _explicitTransaction = context.Database.CurrentTransaction != null;
        var entries = context.ChangeTracker.Entries().ToList();
        foreach (var entry in entries.Where(x => x.Entity is Entity))
        {
            var entity = (Entity)entry.Entity;
            foreach (var domainEvent in entity.DomainEvents)
            {
                if (context.Database.AutoTransactionBehavior == AutoTransactionBehavior.Never || System.Transactions.Transaction.Current != null)
                    throw new InvalidOperationException("Domain events require EF-managed local transactions.");

                if (!_staged.TryGetValue(domainEvent, out var staged))
                {
                    var key = entry.Metadata.FindPrimaryKey();
                    var stream = entry.Metadata.Name + ":" + JsonSerializer.Serialize(key?.Properties.Select(p => entry.Property(p.Name).CurrentValue).ToArray());
                    var message = OutboxMessage.Create(domainEvent, stream, context.Database.ProviderName);
                    staged = (entity, message);
                    _staged.Add(domainEvent, staged);
                    context.Add(message);
                }
                // A retry can change event data (e.g. the synchronization module's recalculated index).
                if (context.Entry(staged.Message).State == EntityState.Added)
                    staged.Message.Payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType());
            }
        }
        // EF may reorder INSERTs by primary key. Preserve the event order independently
        // of the database-generated sequence within this SaveChanges batch.
        var saveId = Guid.NewGuid();
        var position = 0;
        foreach (var message in context.ChangeTracker.Entries<OutboxMessage>().Where(x => x.State == EntityState.Added))
        {
            message.Entity.SaveId = saveId;
            message.Entity.Position = position++;
        }
        if (context.Database.CurrentTransaction != null)
        {
            foreach (var entry in context.ChangeTracker.Entries().Where(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
                _saved.TryAdd(entry.Entity, new SavedState(entry, entry.State, entry.OriginalValues.Clone(), entry.CurrentValues.Clone(), entry.Properties.Where(p => p.IsTemporary).Select(p => p.Metadata.Name).ToHashSet(),
                    entry.Properties.Where(p => p.IsModified).Select(p => p.Metadata.Name).ToHashSet()));
        }
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        CompleteSave(eventData.Context!);
        return result;
    }

    public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        CompleteSave(eventData.Context!);
        return ValueTask.FromResult(result);
    }

    private void CompleteSave(DbContext context)
    {
        foreach (var (domainEvent, (_, message)) in _staged)
        {
            _persisted.Add(domainEvent);
            // SaveChanges(false) must not insert an already persisted outbox row again.
            context.Entry(message).State = EntityState.Unchanged;
        }
        if (!_explicitTransaction)
            Complete();
    }

    private void Complete()
    {
        foreach (var group in _staged.Where(x => _persisted.Contains(x.Key)).GroupBy(x => x.Value.Entity))
            group.Key.RemoveDomainEvents(group.Select(x => x.Key));
        foreach (var domainEvent in _persisted)
            _staged.Remove(domainEvent);
        _persisted.Clear();
        _saved.Clear();
    }

    private void Rollback()
    {
        foreach (var (entry, state, original, current, temporary, modified) in _saved.Values)
        {
            entry.State = state;
            entry.OriginalValues.SetValues(original);
            if (state == EntityState.Modified)
            {
                foreach (var property in entry.Properties)
                    property.IsModified = modified.Contains(property.Metadata.Name);
            }
            if (state == EntityState.Added)
            {
                foreach (var property in entry.Properties.Where(p => p.Metadata.ValueGenerated != Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never))
                {
                    property.CurrentValue = current[property.Metadata.Name];
                    property.IsTemporary = temporary.Contains(property.Metadata.Name);
                }
            }
        }
        _saved.Clear();
        _persisted.Clear();
    }

    private void Commit()
    {
        if (_explicitTransaction)
            Complete();
    }

    private sealed class OutboxTransactionInterceptor(OutboxSaveChangesInterceptor owner) : DbTransactionInterceptor
    {
        public override void TransactionCommitted(DbTransaction transaction, TransactionEndEventData eventData) => owner.Commit();
        public override Task TransactionCommittedAsync(DbTransaction transaction, TransactionEndEventData eventData, CancellationToken cancellationToken = default)
        {
            owner.Commit();
            return Task.CompletedTask;
        }
        public override void TransactionRolledBack(DbTransaction transaction, TransactionEndEventData eventData) => owner.Rollback();
        public override Task TransactionRolledBackAsync(DbTransaction transaction, TransactionEndEventData eventData, CancellationToken cancellationToken = default)
        {
            owner.Rollback();
            return Task.CompletedTask;
        }
        public override void TransactionFailed(DbTransaction transaction, TransactionErrorEventData eventData) => owner.Rollback();
        public override Task TransactionFailedAsync(DbTransaction transaction, TransactionErrorEventData eventData, CancellationToken cancellationToken = default)
        {
            owner.Rollback();
            return Task.CompletedTask;
        }
    }
}
