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
    private int _numberOfModificationsWithoutPayload;

    public SynchronizationDbContextSeeder(IServiceProvider serviceProvider, ILogger<SynchronizationDbContextSeeder> logger)
    {
        _blobStorage = serviceProvider.GetService<IBlobStorage>();
        _blobRootFolder = serviceProvider.GetService<IOptions<BlobOptions>>()!.Value.RootFolder;
        _logger = logger;
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
                .OrderBy(m => m.Index)
                .Skip(_numberOfModificationsWithoutPayload)
                .Take(PAGE_SIZE)
                .ToList();

            var blobReferences = modificationsWithoutEncryptedPayload
                .Where(m => !string.IsNullOrWhiteSpace(m.BlobReference))
                .Select(m => m.BlobReference)
                .Distinct()
                .ToList();

            var blobsFromReferences = await FindBlobsByReferences(blobReferences);

            await FillPayloads(context, modificationsWithoutEncryptedPayload, blobsFromReferences);

            await context.SaveChangesAsync();

            hasMorePages = modificationsWithoutEncryptedPayload.Count != 0;
        }
    }

    private async Task<Dictionary<string, Dictionary<long, byte[]>>> FindBlobsByReferences(IEnumerable<string> blobReferences)
    {
        var blobs = await Task.WhenAll(blobReferences.Select(async r =>
        {
            try
            {
                var blobFromReference = await _blobStorage!.FindAsync(_blobRootFolder!, r);
                return new KeyValuePair<string, byte[]?>(r, blobFromReference);
            }
            catch (NotFoundException)
            {
                return new KeyValuePair<string, byte[]?>(r, null);
            }
        }));

        var deserialized = blobs
            .Where(b => b.Value != null)
            .Select(b => new KeyValuePair<string, Dictionary<long, byte[]>>(b.Key, JsonSerializer.Deserialize<Dictionary<long, byte[]>>(b.Value!)!))
            .ToDictionary(b => b.Key, b => b.Value);

        return deserialized;
    }

    private async Task FillPayloads(SynchronizationDbContext context, List<DatawalletModification> modifications, Dictionary<string, Dictionary<long, byte[]>> blobsFromReferences)
    {
        await Task.WhenAll(modifications.Select(async m => await FillPayload(context, m, blobsFromReferences)));
    }

    private async Task FillPayload(SynchronizationDbContext context, DatawalletModification modification, Dictionary<string, Dictionary<long, byte[]>> blobsFromReferences)
    {
        var hadContent = await FillPayload(modification, blobsFromReferences);

        if (hadContent)
            context.DatawalletModifications.Update(modification);
        else
            Interlocked.Increment(ref _numberOfModificationsWithoutPayload);
    }

    private async Task<bool> FillPayload(DatawalletModification modification, Dictionary<string, Dictionary<long, byte[]>> blobsFromReferences)
    {
        if (string.IsNullOrWhiteSpace(modification.BlobReference))
        {
            // fill via blob id
            try
            {
                var blobContent = await _blobStorage!.FindAsync(_blobRootFolder!, modification.Id);
                modification.LoadEncryptedPayload(blobContent);
            }
            catch (NotFoundException)
            {
                _logger.LogInformation("Blob with Id '{id}' not found. As the encrypted payload of a datawallet modification is not required, this is probably not an error.", modification.Id);
                return false;
            }
        }

        // fill via blob reference
        if (!blobsFromReferences.TryGetValue(modification.BlobReference, out var blob))
        {
            _logger.LogError("Blob with reference '{blobReference}' not found.", modification.BlobReference);
            return false;
        }

        if (!blob.TryGetValue(modification.Index, out var payload))
        {
            _logger.LogInformation("Blob with Id '{id}' not found in blob reference. As the encrypted payload of a datawallet modification is not required, this is probably not an error.", modification.Id);
            return false;
        }

        modification.LoadEncryptedPayload(payload);

        return true;
    }
}
