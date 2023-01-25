using AutoFixture.Dsl;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Files.Domain.Entities;

namespace Files.Application.Tests.Extensions;

public static class FileMetadataPostprocessComposerExtensions
{
    public static IPostprocessComposer<FileMetadata> WithFileId(this IPostprocessComposer<FileMetadata> composer, FileId id)
    {
        return composer.With(f => f.Id, id);
    }

    public static IPostprocessComposer<FileMetadata> ExpiringAt(this IPostprocessComposer<FileMetadata> composer, DateTime expiresAt)
    {
        return composer.With(f => f.ExpiresAt, expiresAt);
    }

    public static IPostprocessComposer<FileMetadata> WithCreatedBy(this IPostprocessComposer<FileMetadata> composer, IdentityAddress createdBy)
    {
        return composer.With(f => f.CreatedBy, createdBy);
    }
}
