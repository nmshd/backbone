using Microsoft.EntityFrameworkCore;
using Synchronization.Infrastructure.Persistence.Database;

namespace Synchronization.Application.Tests;

public class ApplicationDbContextWithDelayedSave : ApplicationDbContext
{
    private readonly TimeSpan _delay;

    public ApplicationDbContextWithDelayedSave(DbContextOptions<ApplicationDbContext> options, TimeSpan delay) : base(options)
    {
        _delay = delay;
    }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        await Task.Delay(_delay, cancellationToken);

        return await base.SaveChangesAsync(cancellationToken);
    }
}
