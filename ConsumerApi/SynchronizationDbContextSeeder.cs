using System.Text.Json;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.Entities;
using Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;
using Backbone.Tooling.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Backbone.ConsumerApi;

public class SynchronizationDbContextSeeder : IDbSeeder<SynchronizationDbContext>
{
    private readonly IBlobStorage? _blobStorage;
    private readonly string? _blobRootFolder;

    public SynchronizationDbContextSeeder(IServiceProvider serviceProvider)
    {
        _blobStorage = serviceProvider.GetService<IBlobStorage>();
        _blobRootFolder = serviceProvider.GetService<IOptions<BlobOptions>>()!.Value.RootFolder;
    }

    public async Task SeedAsync(SynchronizationDbContext context)
    {
        await FillEncryptedPayloadColumnFromBlobStorage(context);
    }

    private async Task FillEncryptedPayloadColumnFromBlobStorage(SynchronizationDbContext context)
    {
        // _blobRootFolder is null when blob storage configuration is not provided, meaning the content of database entries should not be loaded from blob storage
        if (_blobRootFolder == null)
            return;

        var modificationsWithMissingContent = await context.DatawalletModifications
            .Where(t => t.EncryptedPayload == null)
            .ToListAsync();

        // fetching the blob storage content for modifications whose encrypted content is saved in bulk and which have blob reference value
        await FillEncryptedContentForModificationsWithBlobReference(context, modificationsWithMissingContent);

        // fetching the blob storage content for modifications whose encrypted content is saved individually
        await FillEncryptedContentForModificationsWithoutBlobReference(context, modificationsWithMissingContent);
    }

    private async Task FillEncryptedContentForModificationsWithBlobReference(SynchronizationDbContext context, List<DatawalletModification> modificationsWithMissingContent)
    {
        var modificationsWithBlobReferences = modificationsWithMissingContent
            .Where(m => !m.BlobReference.IsEmpty()).ToList();

        var blobReferences = modificationsWithBlobReferences
            .Select(m => m.BlobReference).Distinct();

        var blobs = await Task.WhenAll(blobReferences.Select(r =>
        {
            try
            {
                return _blobStorage!.FindAsync(_blobRootFolder, r);
            }
            catch (NotFoundException)
            {
                throw new Exception($"Blob with reference '{r}' not found.");
            }
        }));

        var payloads = blobs
            .Select(b => JsonSerializer.Deserialize<Dictionary<long, byte[]>>(b))
            .SelectMany(b => b)
            .ToDictionary(b => b.Key, b => b.Value);

        foreach (var datawalletModification in modificationsWithMissingContent)
        {
            if (payloads.TryGetValue(datawalletModification.Index, out var payload))
            {
                datawalletModification.LoadEncryptedPayload(payload);
                context.DatawalletModifications.Update(datawalletModification);
            }
        }

        await context.SaveChangesAsync();
    }

    private async Task FillEncryptedContentForModificationsWithoutBlobReference(SynchronizationDbContext context, List<DatawalletModification> modificationsWithMissingContent)
    {
        var modificationsWithoutBlobReferences = modificationsWithMissingContent.Where(m => m.BlobReference.IsEmpty()).ToList();

        foreach (var datawalletModification in modificationsWithoutBlobReferences)
        {
            try
            {
                var blobContent = await _blobStorage!.FindAsync(_blobRootFolder!, datawalletModification.Id);
                datawalletModification.LoadEncryptedPayload(blobContent);
                context.DatawalletModifications.Update(datawalletModification);
            }
            catch (NotFoundException)
            {
                // due to missing validation, it was possible to create a relationship template without request content;
                // therefore we cannot tell whether this exception is an error or not
            }
        }

        await context.SaveChangesAsync();
    }
}
