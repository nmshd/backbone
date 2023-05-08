using Microsoft.EntityFrameworkCore;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.Extensions.Options;
using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Files.Application.Infrastructure.Persistence;
using Backbone.Modules.Files.Domain.Entities;
using Backbone.Modules.Files.Application.Extensions;

namespace Backbone.Modules.Files.Infrastructure.Persistence.Database.Repository;
public class FilesRepository : IFilesRepository
{
    private readonly DbSet<FileMetadata> _files;
    private readonly IQueryable<FileMetadata> _readOnlyFiles;
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

    public async Task<FileMetadata> FindById(FileId fileId)
    {
        var file = _readOnlyFiles
            .WithId(fileId)
            .NotExpired()
            .NotDeleted()
            .FirstOrDefault();

        var fileContent = await _blobStorage.FindAsync(_blobOptions.RootFolder, fileId);
        file.LoadContent(fileContent);

        return file;
    }
}
