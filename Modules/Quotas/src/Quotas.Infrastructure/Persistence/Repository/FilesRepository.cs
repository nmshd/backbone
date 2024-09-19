using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.FileMetadata;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;
public class FilesRepository : IFilesRepository
{
    private readonly IQueryable<FileMetadata> _readOnlyFiles;

    public FilesRepository(QuotasDbContext dbContext)
    {
        _readOnlyFiles = dbContext.Files.AsNoTracking();
    }

    public async Task<uint> Count(string uploader, DateTime createdAtFrom, DateTime createdAtTo, CancellationToken cancellationToken)
    {
        var count = await _readOnlyFiles
            .CreatedInInterval(createdAtFrom, createdAtTo)
            .CountAsync(f => f.CreatedBy == uploader, cancellationToken);
        return (uint)count;
    }

    public async Task<long> AggregateUsedSpace(string uploader, DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        var totalSpace = await _readOnlyFiles
            .CreatedInInterval(from, to)
            .Where(f => f.CreatedBy == uploader).SumAsync(f => f.CipherSize, cancellationToken);
        return totalSpace;
    }
}
