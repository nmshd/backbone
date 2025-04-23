using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Files.Domain.Entities;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;

public interface IFilesRepository
{
    Task<File?> Find(FileId id, CancellationToken cancellationToken, bool track = false, bool fillContent = true);
    Task<DbPaginationResult<File>> FindFilesByCreator(IEnumerable<FileId> fileIds, IdentityAddress creatorAddress, PaginationFilter paginationFilter, CancellationToken cancellationToken);
    Task Add(File file, CancellationToken cancellationToken);
    Task Delete(File file, CancellationToken cancellationToken);
    Task DeleteFilesOfIdentity(Expression<Func<File, bool>> filter, CancellationToken cancellationToken);
    void Update(File file, CancellationToken cancellationToken);
}
