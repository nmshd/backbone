using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Text;
using Xunit;

namespace Enmeshed.BuildingBlocks.Infrastructure.Tests.Tests;

[Collection("AzureBlobStorageTests")]
public class AzureStorageAccountTests
{
    private static void StartAzuriteContainer()
    {
        var processInfo = new ProcessStartInfo("docker",
            $"run --rm --name azurite-test-container -p 10000:10000 mcr.microsoft.com/azure-storage/azurite azurite-blob --blobHost 0.0.0.0")
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using var process = new Process();
        process.StartInfo = processInfo;
        process.Start();
        process.WaitForExit(10000);
        if (!process.HasExited)
        {
            process.Kill();
        }

        process.Close();
    }

    private static void CloseAzuriteContainer()
    {
        var processInfo = new ProcessStartInfo("docker", $"stop azurite-test-container")
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using var process = new Process();
        process.StartInfo = processInfo;
        process.Start();
        process.WaitForExit(10000);
        if (!process.HasExited)
        {
            process.Kill();
        }

        process.Close();
    }

    private static IBlobStorage ProvisionAzureStorageTests()
    {
        StartAzuriteContainer();

        var services = new ServiceCollection()
            .AddLogging();

        var connectionString = "UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://127.0.0.1;";
        var containerName = "azureblobstoragecontainer";

        Action<AzureStorageAccountOptions> options = x =>
        {
            x.ContainerName = containerName;
            x.ConnectionString = connectionString;
        };

        services.AddAzureStorageAccount(options);

        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetService<IBlobStorage>();
    }

    [Fact]
    public async Task AzureSaveAsyncAndFindAsync()
    {
        var azureBlobStorage = ProvisionAzureStorageTests();

        var addBlobName = "AzureSaveAsyncAndFindAsync";
        var addBlobContent = Encoding.ASCII.GetBytes("AzureSaveAsyncAndFindAsync");

        azureBlobStorage.Add(addBlobName, addBlobContent);
        await azureBlobStorage.SaveAsync();

        var retrievedBlobContent = await azureBlobStorage.FindAsync(addBlobName);
        Assert.Equal(addBlobContent, retrievedBlobContent);

        CloseAzuriteContainer();
    }

    [Fact]
    public async Task AzureDeleteBlobThatExists()
    {
        var azureBlobStorage = ProvisionAzureStorageTests();

        var addBlobName = "AzureDeleteBlobThatExists";
        var addBlobContent = Encoding.ASCII.GetBytes("AzureDeleteBlobThatExists");

        azureBlobStorage.Add(addBlobName, addBlobContent);
        azureBlobStorage.Remove(addBlobName);
        await azureBlobStorage.SaveAsync();

        CloseAzuriteContainer();
    }

    [Fact]
    public async Task AzureDeleteBlobThatDoesNotExist()
    {
        var azureBlobStorage = ProvisionAzureStorageTests();

        var blobName = "AzureDeleteBlobThatDoesNotExist";
        azureBlobStorage.Remove(blobName);

        await Assert.ThrowsAsync<NotFoundException>(async () => await azureBlobStorage.SaveAsync());

        CloseAzuriteContainer();
    }

    [Fact]
    public async Task AzureAddBlobWithSameName()
    {
        var azureBlobStorage = ProvisionAzureStorageTests();

        var addBlobName = "AzureAddBlobWithSameName";
        var addBlobContent = Encoding.ASCII.GetBytes("AddBlobWithSameName Before.");
        azureBlobStorage.Add(addBlobName, addBlobContent);

        addBlobContent = Encoding.ASCII.GetBytes("AddBlobWithSameName After.");
        azureBlobStorage.Add(addBlobName, addBlobContent);

        await Assert.ThrowsAsync<BlobAlreadyExistsException>(async () => await azureBlobStorage.SaveAsync());

        CloseAzuriteContainer();
    }

    [Fact]
    public async Task AzureAddMultipleBlobsAndFindAllBlobs()
    {
        var azureBlobStorage = ProvisionAzureStorageTests();

        var addBlobName1 = "AzureAddMultipleBlobsAndFindAllBlobs1";
        var addBlobName2 = "AzureAddMultipleBlobsAndFindAllBlobs2";

        var addBlobContent1 = Encoding.ASCII.GetBytes("AzureAddMultipleBlobsAndFindAllBlobs1");
        var addBlobContent2 = Encoding.ASCII.GetBytes("AzureAddMultipleBlobsAndFindAllBlobs2");

        azureBlobStorage.Add(addBlobName1, addBlobContent1);
        azureBlobStorage.Add(addBlobName2, addBlobContent2);
        await azureBlobStorage.SaveAsync();

        var retrievedBlobContent = await azureBlobStorage.FindAllAsync();

        Assert.Contains<string>(addBlobName1, retrievedBlobContent);
        Assert.Contains<string>(addBlobName2, retrievedBlobContent);

        CloseAzuriteContainer();
    }
}