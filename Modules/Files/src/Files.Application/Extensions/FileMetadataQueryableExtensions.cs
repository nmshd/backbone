using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Files.Application.Files.DTOs;
using Files.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Files.Application.Extensions;

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

    public static async Task<FileMetadataDTO> FirstWithId(this IQueryable<FileMetadataDTO> query, FileId id, CancellationToken cancellationToken)
    {
        var challenge = await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (challenge == null)
            throw new NotFoundException("File");

        return challenge;
    }
}
