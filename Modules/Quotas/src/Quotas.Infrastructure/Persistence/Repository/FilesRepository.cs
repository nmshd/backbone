using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.FileMetadata;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
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
        var count = await _readOnlyFiles.CountAsync(f => f.CreatedBy == uploader, cancellationToken);
        return (uint)count;
    }

    public async Task<uint> UsedSpace(string uploader, DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        var totalSpace = await _readOnlyFiles.Where(f => f.CreatedBy == uploader).SumAsync(f => (long)f.CipherSize, cancellationToken);
        return (uint)totalSpace;
    }
}
