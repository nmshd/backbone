using Backbone.Modules.Files.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
public interface IFilesRepository
{
    Task<File> Find(FileId id, CancellationToken cancellationToken, bool track = false, bool fillContent = true);
    Task<DbPaginationResult<File>> FindFilesByCreator(IEnumerable<FileId> fileIds, IdentityAddress creatorAddress, PaginationFilter paginationFilter, CancellationToken cancellationToken);
    Task Add(File file, CancellationToken cancellationToken);
}
