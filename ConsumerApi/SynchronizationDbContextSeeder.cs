using System.Linq;
using System.Text.Json;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.Entities;
using Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.Options;

namespace Backbone.ConsumerApi;

public class SynchronizationDbContextSeeder : IDbSeeder<SynchronizationDbContext>
{
    private const int PAGE_SIZE = 500;

    private readonly IBlobStorage? _blobStorage;
    private readonly string? _blobRootFolder;
    private readonly ILogger<SynchronizationDbContextSeeder> _logger;

    private int _modificationsWithNoPayloadInBlobStorageCount;

    public SynchronizationDbContextSeeder(IServiceProvider serviceProvider, ILogger<SynchronizationDbContextSeeder> logger)
    {
        _blobStorage = serviceProvider.GetService<IBlobStorage>();
        _blobRootFolder = serviceProvider.GetService<IOptions<BlobOptions>>()!.Value.RootFolder;
        _logger = logger;

        _modificationsWithNoPayloadInBlobStorageCount = 0;
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

        var hasMorePages = true;

        while (hasMorePages)
        {
            var modificationsWithoutEncryptedPayload = context.DatawalletModifications
                .Where(m => m.EncryptedPayload == null)
                .OrderBy(m => m.Id)
                .Skip(_modificationsWithNoPayloadInBlobStorageCount)
                .Take(PAGE_SIZE)
                .ToList();

            foreach (var datawalletModification in modificationsWithoutEncryptedPayload)
                await FillEncryptedContentForModification(context, datawalletModification);

            hasMorePages = modificationsWithoutEncryptedPayload.Any();
        }
    }

    private async Task FillEncryptedContentForModification(SynchronizationDbContext context, DatawalletModification datawalletModification)
    {
        if (datawalletModification.BlobReference.Trim().IsEmpty())
        {
            // fetching the blob storage content for modifications whose encrypted content is saved individually and which do not have blob reference value
            try
            {
                var blobContent = await _blobStorage!.FindAsync(_blobRootFolder!, datawalletModification.Id);
                datawalletModification.LoadEncryptedPayload(blobContent);
                context.DatawalletModifications.Update(datawalletModification);
            }
            catch (NotFoundException)
            {
                _modificationsWithNoPayloadInBlobStorageCount++;
                _logger.LogInformation($"Blob with reference '{datawalletModification.BlobReference}' not found.");

                // The encrypted payload of a datawallet modification is not required.
                // Therefore we cannot tell whether this exception is an error or not
            }
        }
        else
        {
            // fetching the blob storage content for modifications whose encrypted content is saved in bulk and which have blob reference value
            try
            {
                var blob = await _blobStorage!.FindAsync(_blobRootFolder!, datawalletModification.BlobReference);
                var payload = JsonSerializer.Deserialize<Dictionary<long, byte[]>>(blob);

                if (payload != null && payload.TryGetValue(datawalletModification.Index, out var encryptedPayload))
                {
                    datawalletModification.LoadEncryptedPayload(encryptedPayload);
                    context.DatawalletModifications.Update(datawalletModification);
                }
                else
                {
                    _modificationsWithNoPayloadInBlobStorageCount++;
                }
            }
            catch (NotFoundException)
            {
                _modificationsWithNoPayloadInBlobStorageCount++;
                _logger.LogInformation($"Blob with reference '{datawalletModification.BlobReference}' not found.");
            }
        }

        await context.SaveChangesAsync();
    }
}
