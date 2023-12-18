using System.Text.Json;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.Entities;
using Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;
using Backbone.Tooling.Extensions;
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

        var paginationResult = await context.DatawalletModifications
            .Where(t => t.EncryptedPayload == null)
            .OrderAndPaginate(m => m.Id, new PaginationFilter(1, 20), CancellationToken.None);

        foreach (var datawalletModification in paginationResult.ItemsOnPage)
            await FillEncryptedContentForModification(context, datawalletModification);
    }

    private async Task FillEncryptedContentForModification(SynchronizationDbContext context, DatawalletModification datawalletModification)
    {
        if (datawalletModification.BlobReference.Trim().IsEmpty())
        {
            try
            {
                var blobContent = await _blobStorage!.FindAsync(_blobRootFolder!, datawalletModification.Id);
                datawalletModification.LoadEncryptedPayload(blobContent);
                context.DatawalletModifications.Update(datawalletModification);
            }
            catch (NotFoundException)
            {
                // The encrypted payload of a datawallet modification is not required.
                // Therefore we cannot tell whether this exception is an error or not
            }
        }
        else
        {
            try
            {
                var blob = await _blobStorage!.FindAsync(_blobRootFolder, datawalletModification.BlobReference);
                var payload = JsonSerializer.Deserialize<Dictionary<long, byte[]>>(blob);

                if (payload.TryGetValue(datawalletModification.Index, out var encryptedPayload))
                {
                    datawalletModification.LoadEncryptedPayload(encryptedPayload);
                    context.DatawalletModifications.Update(datawalletModification);
                }
            }
            catch (NotFoundException)
            {
                throw new Exception($"Blob with reference '{datawalletModification.BlobReference}' not found.");
            }
        }

        await context.SaveChangesAsync();
    }
}
