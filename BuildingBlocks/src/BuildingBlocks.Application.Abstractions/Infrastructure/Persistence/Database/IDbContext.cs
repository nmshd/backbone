using System.Data;
using Microsoft.EntityFrameworkCore;

namespace Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;

public interface IDbContext
{
    DbSet<T> Set<T>() where T : class;
    IQueryable<T> SetReadOnly<T>() where T : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    Task RunInTransaction(Func<Task> action, List<int> errorNumbersToRetry,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

    Task RunInTransaction(Func<Task> action, IsolationLevel isolationLevel);

    Task<T> RunInTransaction<T>(Func<Task<T>> action, List<int>? errorNumbersToRetry,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

    Task<T> RunInTransaction<T>(Func<Task<T>> func, IsolationLevel isolationLevel);
}
