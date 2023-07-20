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

    public async Task<uint> Count(IdentityAddress createdBy, DateTime createdAtFrom, DateTime createdAtTo, CancellationToken cancellationToken)
    {
        var count = await _readOnlyFiles.CountAsync(f => f.CreatedBy == createdBy.StringValue, cancellationToken);
        return (uint)count;
    }
}
