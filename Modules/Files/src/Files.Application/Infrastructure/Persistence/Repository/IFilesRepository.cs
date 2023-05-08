using Backbone.Modules.Files.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
public interface IFilesRepository
{
    Task<FileMetadata> Find(FileId id, bool track = false, bool fillContent = true);
    Task<DbPaginationResult<FileMetadata>> FindFilesByCreator(IEnumerable<FileId> fileIds, IdentityAddress creatorAddress, PaginationFilter paginationFilter);
}
