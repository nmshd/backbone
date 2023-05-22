using Backbone.Modules.Files.Domain.Entities;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Infrastructure.Persistence.Database.QueryableExtensions;

public static class FileQueryableExtensions
{
    public static IQueryable<File> WithIdIn(this IQueryable<File> query, IEnumerable<FileId> ids)
    {
        return query.Where(e => ids.Contains(e.Id));
    }

    public static IQueryable<File> CreatedBy(this IQueryable<File> query, IdentityAddress createdBy)
    {
        return query.Where(e => e.CreatedBy == createdBy);
    }

    public static IQueryable<File> NotExpired(this IQueryable<File> query)
    {
        return query.Where(File.IsNotExpired);
    }

    public static IQueryable<File> NotDeleted(this IQueryable<File> query)
    {
        return query.Where(File.IsNotDeleted);
    }

    public static IQueryable<File> WithId(this IQueryable<File> query, FileId id)
    {
        return query.Where(e => e.Id == id);
    }
}
