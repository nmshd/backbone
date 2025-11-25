using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Files.Domain.Entities;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Infrastructure.Persistence.Database.QueryableExtensions;

public static class FileQueryableExtensions
{
    extension(IQueryable<File> query)
    {
        public IQueryable<File> WithIdIn(IEnumerable<FileId> ids)
        {
            return query.Where(e => ids.Contains(e.Id));
        }

        public IQueryable<File> CreatedBy(IdentityAddress createdBy)
        {
            return query.Where(e => e.CreatedBy == createdBy);
        }

        public IQueryable<File> NotExpired()
        {
            return query.Where(File.IsNotExpired);
        }

        public IQueryable<File> NotDeleted()
        {
            return query.Where(File.IsNotDeleted);
        }

        public IQueryable<File> WithId(FileId id)
        {
            return query.Where(e => e.Id == id);
        }
    }
}
