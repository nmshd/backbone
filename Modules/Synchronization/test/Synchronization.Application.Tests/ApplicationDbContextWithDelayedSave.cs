using Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Synchronization.Application.Tests;

public class ApplicationDbContextWithDelayedSave : SynchronizationDbContext
{
    private readonly TimeSpan _delay;

    public ApplicationDbContextWithDelayedSave(DbContextOptions<SynchronizationDbContext> options, TimeSpan delay) : base(options, A.Fake<IServiceProvider>())
    {
        _delay = delay;
    }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        await Task.Delay(_delay, cancellationToken);

        return await base.SaveChangesAsync(cancellationToken);
    }
}
