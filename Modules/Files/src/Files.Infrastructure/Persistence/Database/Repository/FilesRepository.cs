using Microsoft.EntityFrameworkCore;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.Extensions.Options;
using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Files.Application.Infrastructure.Persistence;
using Backbone.Modules.Files.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using File = Backbone.Modules.Files.Domain.Entities.File;
using Backbone.Modules.Files.Infrastructure.Persistence.Database.QueryableExtensions;

namespace Backbone.Modules.Files.Infrastructure.Persistence.Database.Repository;
public class FilesRepository : IFilesRepository
{
    private readonly DbSet<File> _files;
    private readonly IQueryable<File> _readOnlyFiles;
    private readonly FilesDbContext _dbContext;
    private readonly IBlobStorage _blobStorage;
    private readonly BlobOptions _blobOptions;

    public FilesRepository(FilesDbContext dbContext, IBlobStorage blobStorage, IOptions<BlobOptions> blobOptions)
    {
        _files = dbContext.FileMetadata;
        _readOnlyFiles = dbContext.FileMetadata.AsNoTracking();
        _dbContext = dbContext;
        _blobStorage = blobStorage;
        _blobOptions = blobOptions.Value;
    }

    public async Task Add(File file, CancellationToken cancellationToken)
    {
        _blobStorage.Add(_blobOptions.RootFolder, file.Id, file.Content);
        await _files.AddAsync(file, cancellationToken);
        await _blobStorage.SaveAsync();
        await _dbContext.SaveChangesAsync(cancellationToken);

    }

    public async Task<File> Find(FileId fileId, bool track = false, bool fillContent = true)
    {
        var file = (track ? _files : _readOnlyFiles)
            .WithId(fileId)
            .NotExpired()
            .NotDeleted()
            .FirstOrDefault();

        if (fillContent)
        {
            var fileContent = await _blobStorage.FindAsync(_blobOptions.RootFolder, fileId);
            file.LoadContent(fileContent);
        }

        return file;
    }

    public async Task<DbPaginationResult<File>> FindFilesByCreator(IEnumerable<FileId> fileIds, IdentityAddress creatorAddress, PaginationFilter paginationFilter)
    {
        var query = _dbContext
            .SetReadOnly<File>()
            .CreatedBy(creatorAddress)
            .NotExpired()
            .NotDeleted();

        if (fileIds.Any())
            query = query.WithIdIn(fileIds);

        return await query.OrderAndPaginate(d => d.CreatedAt, paginationFilter);
    }
}
