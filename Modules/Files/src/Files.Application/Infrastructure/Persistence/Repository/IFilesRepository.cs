using Backbone.Modules.Files.Domain.Entities;

namespace Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
public interface IFilesRepository
{
    Task<FileMetadata> FindById(FileId id);
}
