using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage.AzureStorageAccount;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.Azurite;

namespace Backbone.BuildingBlocks.Infrastructure.Tests.Tests;

[Collection("AzureBlobStorageTests")]
public class AzureStorageAccountTests : AbstractTestsBase, IAsyncLifetime
{
    private static AzuriteContainer _container = null!;
    private const string CONTAINER_NAME = "test-container";

    public async ValueTask InitializeAsync()
    {
        _container = new AzuriteBuilder("mcr.microsoft.com/azure-storage/azurite:latest").WithCommand("--skipApiVersionCheck").WithExposedPort(10000).Build();

        await _container.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _container.StopAsync();
    }

    [Fact]
    public async Task AzureSaveAsyncAndFindAsync()
    {
        var azureBlobStorage = ProvisionAzureStorageTests();

        const string addBlobName = "AzureSaveAsyncAndFindAsync";
        var addBlobContent = "AzureSaveAsyncAndFindAsync"u8.ToArray();

        azureBlobStorage.Add(CONTAINER_NAME, addBlobName, addBlobContent);
        await azureBlobStorage.SaveAsync();

        var retrievedBlobContent = await azureBlobStorage.GetAsync(CONTAINER_NAME, addBlobName);
        addBlobContent.ShouldBe(retrievedBlobContent);
    }

    [Fact]
    public async Task AzureDeleteBlobThatExists()
    {
        var azureBlobStorage = ProvisionAzureStorageTests();

        const string addBlobName = "AzureDeleteBlobThatExists";
        var addBlobContent = "AzureDeleteBlobThatExists"u8.ToArray();

        azureBlobStorage.Add(CONTAINER_NAME, addBlobName, addBlobContent);

        azureBlobStorage.Remove(CONTAINER_NAME, addBlobName);
        var acting = azureBlobStorage.SaveAsync;

        await acting.ShouldNotThrowAsync();
    }

    [Fact]
    public async Task AzureDeleteBlobThatDoesNotExist()
    {
        var azureBlobStorage = ProvisionAzureStorageTests();

        azureBlobStorage.Remove(CONTAINER_NAME, "AzureDeleteBlobThatDoesNotExist");
        var acting = azureBlobStorage.SaveAsync;

        await acting.ShouldNotThrowAsync();
    }

    [Fact]
    public async Task AzureAddBlobWithSameName()
    {
        var azureBlobStorage = ProvisionAzureStorageTests();

        const string addBlobName = "AzureAddBlobWithSameName";

        azureBlobStorage.Add(CONTAINER_NAME, addBlobName, "AddBlobWithSameName Before"u8.ToArray());
        azureBlobStorage.Add(CONTAINER_NAME, addBlobName, "AddBlobWithSameName After"u8.ToArray());

        var acting = azureBlobStorage.SaveAsync;

        await acting.ShouldThrowAsync<BlobAlreadyExistsException>();
    }

    [Fact]
    public async Task AzureAddMultipleBlobsAndFindAllBlobs()
    {
        var azureBlobStorage = ProvisionAzureStorageTests();

        const string addBlobName1 = "AzureAddMultipleBlobsAndFindAllBlobs1";
        const string addBlobName2 = "AzureAddMultipleBlobsAndFindAllBlobs2";

        var addBlobContent1 = "AzureAddMultipleBlobsAndFindAllBlobs1"u8.ToArray();
        var addBlobContent2 = "AzureAddMultipleBlobsAndFindAllBlobs2"u8.ToArray();

        azureBlobStorage.Add(CONTAINER_NAME, addBlobName1, addBlobContent1);
        azureBlobStorage.Add(CONTAINER_NAME, addBlobName2, addBlobContent2);
        await azureBlobStorage.SaveAsync();

        var retrievedBlobContent = await (await azureBlobStorage.ListAsync(CONTAINER_NAME)).ToListAsync(TestContext.Current.CancellationToken);

        retrievedBlobContent.ShouldContain(addBlobName1);
        retrievedBlobContent.ShouldContain(addBlobName2);
    }

    [Fact]
    public async Task AzureAddMultiplePrefixBlobsAndFindAllBlobs()
    {
        var azureBlobStorage = ProvisionAzureStorageTests();

        azureBlobStorage.Add(CONTAINER_NAME, "PREFIX1-Blob", "content"u8.ToArray());
        azureBlobStorage.Add(CONTAINER_NAME, "PREFIX2-Blob", "content"u8.ToArray());
        await azureBlobStorage.SaveAsync();

        var blobsWithPrefix1 = await (await azureBlobStorage.ListAsync(CONTAINER_NAME, "PREFIX1")).ToListAsync(TestContext.Current.CancellationToken);

        blobsWithPrefix1.ShouldContain("PREFIX1-Blob");
        blobsWithPrefix1.ShouldNotContain("PREFIX2-Blob");
    }

    [Fact]
    public async Task AzureEmptyFindAllBlobs()
    {
        var azureBlobStorage = ProvisionAzureStorageTests();

        var retrievedBlobContent = await (await azureBlobStorage.ListAsync(CONTAINER_NAME)).ToListAsync(TestContext.Current.CancellationToken);

        retrievedBlobContent.ShouldBeEmpty();
    }

    private static IBlobStorage ProvisionAzureStorageTests()
    {
        var services = new ServiceCollection()
            .AddLogging();

        services.AddAzureStorageAccount(new AzureStorageAccountConfiguration
        {
            ConnectionString = _container.GetConnectionString(),
            ContainerName = "test"
        });

        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<IBlobStorage>();
    }
}
