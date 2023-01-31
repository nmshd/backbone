using System.Diagnostics;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Enmeshed.BuildingBlocks.Infrastructure.Tests.Tests;

[Collection("AzureBlobStorageTests")]
public class AzureStorageAccountTests
{
    private static void StartAzuriteContainer()
    {
        var processInfo = new ProcessStartInfo("docker",
            "run --rm --name azurite-test-container -p 10000:10000 mcr.microsoft.com/azure-storage/azurite azurite-blob --blobHost 0.0.0.0")
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using var process = new Process();
        process.StartInfo = processInfo;
        process.Start();
        process.WaitForExit(TimeSpan.FromSeconds(60));
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

        services.AddAzureStorageAccount(x =>
        {
            x.ContainerName = "azureblobstoragecontainer";
            x.ConnectionString = "UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://127.0.0.1;";
        });

        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetService<IBlobStorage>();
    }

    [Fact(Skip = "Fails because emulator container can't be started")]
    public async Task AzureSaveAsyncAndFindAsync()
    {
        var azureBlobStorage = ProvisionAzureStorageTests();

        var addBlobName = "AzureSaveAsyncAndFindAsync";
        var addBlobContent = "AzureSaveAsyncAndFindAsync"u8.ToArray();

        azureBlobStorage.Add(addBlobName, addBlobContent);
        await azureBlobStorage.SaveAsync();

        var retrievedBlobContent = await azureBlobStorage.FindAsync(addBlobName);
        Assert.Equal(addBlobContent, retrievedBlobContent);

        CloseAzuriteContainer();
    }

    [Fact(Skip = "Fails because emulator container can't be started")]
    public async Task AzureDeleteBlobThatExists()
    {
        var azureBlobStorage = ProvisionAzureStorageTests();

        const string ADD_BLOB_NAME = "AzureDeleteBlobThatExists";
        var addBlobContent = "AzureDeleteBlobThatExists"u8.ToArray();

        azureBlobStorage.Add(ADD_BLOB_NAME, addBlobContent);
        azureBlobStorage.Remove(ADD_BLOB_NAME);
        await azureBlobStorage.SaveAsync();

        CloseAzuriteContainer();
    }

    [Fact(Skip = "Fails because emulator container can't be started")]
    public async Task AzureDeleteBlobThatDoesNotExist()
    {
        var azureBlobStorage = ProvisionAzureStorageTests();

        azureBlobStorage.Remove("AzureDeleteBlobThatDoesNotExist");

        await Assert.ThrowsAsync<NotFoundException>(azureBlobStorage.SaveAsync);

        CloseAzuriteContainer();
    }

    [Fact(Skip = "Fails because emulator container can't be started")]
    public async Task AzureAddBlobWithSameName()
    {
        var azureBlobStorage = ProvisionAzureStorageTests();

        const string ADD_BLOB_NAME = "AzureAddBlobWithSameName";

        azureBlobStorage.Add(ADD_BLOB_NAME, "AddBlobWithSameName Before"u8.ToArray());
        azureBlobStorage.Add(ADD_BLOB_NAME, "AddBlobWithSameName After"u8.ToArray());

        await Assert.ThrowsAsync<BlobAlreadyExistsException>(azureBlobStorage.SaveAsync);

        CloseAzuriteContainer();
    }

    [Fact(Skip = "Fails because emulator container can't be started")]
    public async Task AzureAddMultipleBlobsAndFindAllBlobs()
    {
        var azureBlobStorage = ProvisionAzureStorageTests();

        const string ADD_BLOB_NAME1 = "AzureAddMultipleBlobsAndFindAllBlobs1";
        const string ADD_BLOB_NAME2 = "AzureAddMultipleBlobsAndFindAllBlobs2";

        var addBlobContent1 = "AzureAddMultipleBlobsAndFindAllBlobs1"u8.ToArray();
        var addBlobContent2 = "AzureAddMultipleBlobsAndFindAllBlobs2"u8.ToArray();

        azureBlobStorage.Add(ADD_BLOB_NAME1, addBlobContent1);
        azureBlobStorage.Add(ADD_BLOB_NAME2, addBlobContent2);
        await azureBlobStorage.SaveAsync();

        var retrievedBlobContent = await (await azureBlobStorage.FindAllAsync()).ToListAsync();

        retrievedBlobContent.Should().Contain(ADD_BLOB_NAME1);
        retrievedBlobContent.Should().Contain(ADD_BLOB_NAME2);

        CloseAzuriteContainer();
    }
}