using Backbone.Modules.Files.Domain.Entities;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Files.Application.Extensions;

public static class FileMetadataQueryableExtensions
{
    public static IQueryable<FileMetadata> WithIdIn(this IQueryable<FileMetadata> query, IEnumerable<FileId> ids)
    {
        return query.Where(e => ids.Contains(e.Id));
    }

    public static IQueryable<FileMetadata> CreatedBy(this IQueryable<FileMetadata> query, IdentityAddress createdBy)
    {
        return query.Where(e => e.CreatedBy == createdBy);
    }

    public static IQueryable<FileMetadata> NotExpired(this IQueryable<FileMetadata> query)
    {
        return query.Where(FileMetadata.IsNotExpired);
    }

    public static IQueryable<FileMetadata> NotDeleted(this IQueryable<FileMetadata> query)
    {
        return query.Where(FileMetadata.IsNotDeleted);
    }

    public static IQueryable<FileMetadata> WithId(this IQueryable<FileMetadata> query, FileId id)
    {
        return query.Where(e => e.Id == id);
    }
}
