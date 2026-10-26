using System.Reflection;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.Outbox;

public static class InboxDispatcher
{
    public static async Task Handle(IServiceProvider services, Type handlerType, DomainEvent domainEvent)
    {
        var database = services.GetServices<OutboxDatabase>()
            .SingleOrDefault(x => handlerType.Namespace == $"Backbone.Modules.{x.Module}.Module" || handlerType.Namespace?.StartsWith($"Backbone.Modules.{x.Module}.Module.", StringComparison.Ordinal) == true);
        if (database == null)
        {
            await using var scope = services.CreateAsyncScope();
            await Invoke(scope.ServiceProvider, handlerType, domainEvent);
            return;
        }

        await using var strategyScope = services.CreateAsyncScope();
        var strategyContext = (DbContext)strategyScope.ServiceProvider.GetRequiredService(database.ContextType);
        // Joining the inbox transaction moves index-allocation retries out of nested
        // RunInTransaction calls (notably Synchronization.CreateExternalEvent).
        var strategy = strategyContext.Database.ProviderName switch
        {
            "Npgsql.EntityFrameworkCore.PostgreSQL" => (IExecutionStrategy)new NpgsqlRetryingExecutionStrategy(strategyContext, 15, TimeSpan.FromSeconds(1), ["23505"]),
            "Microsoft.EntityFrameworkCore.SqlServer" => new SqlServerRetryingExecutionStrategy(strategyContext, 15, TimeSpan.FromSeconds(1), [2601, 2627]),
            _ => strategyContext.Database.CreateExecutionStrategy()
        };
        await strategy.ExecuteAsync(async () =>
        {
            // Every execution-strategy retry gets fresh handler state and a fresh DbContext.
            await using var scope = services.CreateAsyncScope();
            var context = (DbContext)scope.ServiceProvider.GetRequiredService(database.ContextType);
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var handler = handlerType.FullName!;
                if (await context.Set<InboxMessage>().AnyAsync(x => x.EventId == domainEvent.DomainEventId && x.Handler == handler))
                    return;
                context.Add(new InboxMessage { EventId = domainEvent.DomainEventId, Handler = handler, ProcessedAt = DateTime.UtcNow });
                // Reserve the unique key before side effects. A concurrent duplicate waits here;
                // on conflict the outer retry observes the committed receipt.
                await context.SaveChangesAsync();
                await Invoke(scope.ServiceProvider, handlerType, domainEvent);
                await transaction.CommitAsync();
            }
            catch
            {
                try
                {
                    await transaction.RollbackAsync();
                }
                catch (Exception)
                {
                    // Preserve the original error for the execution strategy/broker retry.
                }
                throw;
            }
        });
    }

    private static async Task Invoke(IServiceProvider services, Type type, DomainEvent domainEvent)
    {
        var handler = services.GetRequiredService(type);
        if (handler is not IDomainEventHandler)
            throw new InvalidOperationException("The consumer does not implement IDomainEventHandler.");
        try
        {
            await (Task)type.GetMethod("Handle")!.Invoke(handler, [domainEvent])!;
        }
        catch (TargetInvocationException exception) when (exception.InnerException != null)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
        }
    }
}
