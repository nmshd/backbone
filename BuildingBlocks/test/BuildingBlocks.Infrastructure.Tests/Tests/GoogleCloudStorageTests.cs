using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.BlobStorage.GoogleCloudStorage;
using FluentAssertions;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Xunit;
using Xunit.Abstractions;

namespace Enmeshed.BuildingBlocks.Infrastructure.Tests.Tests;

public class GoogleCloudStorageTests : IAsyncLifetime
{
    public const string BUCKET_NAME = "test-bucket-nmshd";
    private readonly StorageClient _storageClient;
    private readonly IBlobStorage _blobStorageUnderTest;

    public GoogleCloudStorageTests(ITestOutputHelper output)
    {
        const string authJson = "";

        _storageClient = StorageClient.Create(GoogleCredential.FromJson(authJson));

        var logger = output.BuildLoggerFor<GoogleCloudStorage>();
        _blobStorageUnderTest = new GoogleCloudStorage(_storageClient, logger);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        var blobs = _storageClient.ListObjectsAsync(BUCKET_NAME);

        await foreach (var blob in blobs)
        {
            await _storageClient.DeleteObjectAsync(BUCKET_NAME, blob.Name);
        }
    }

    [Fact(Skip = "No valid emulator for GCP")]
    public async Task SaveAndFindSingleBlob()
    {
        const string blobName = "BlobName";
        var blobContent = "BlobContent".GetBytes();

        _blobStorageUnderTest.Add(BUCKET_NAME, blobName, blobContent);
        await _blobStorageUnderTest.SaveAsync();

        var retrievedBlobContent = await _blobStorageUnderTest.FindAsync(BUCKET_NAME, blobName);
        retrievedBlobContent.Should().Equal(blobContent);
    }

    [Fact(Skip = "No valid emulator for GCP")]
    public async Task SaveAndFindMultipleBlobs()
    {
        var blob1Content = "BlobContent1".GetBytes();
        var blob2Content = "BlobContent2".GetBytes();

        _blobStorageUnderTest.Add(BUCKET_NAME, "BlobName1", blob1Content);
        _blobStorageUnderTest.Add(BUCKET_NAME, "BlobName2", blob2Content);

        await _blobStorageUnderTest.SaveAsync();

        var retrievedBlob1Content = await _blobStorageUnderTest.FindAsync(BUCKET_NAME, "BlobName1");
        var retrievedBlob2Content = await _blobStorageUnderTest.FindAsync(BUCKET_NAME, "BlobName2");

        retrievedBlob1Content.Should().Equal(blob1Content);
        retrievedBlob2Content.Should().Equal(blob2Content);
    }

    [Fact(Skip = "No valid emulator for GCP")]
    public async Task AddBlobWithSameName()
    {
        const string blobName = "AddBlobWithSameName";

        var blobContent = "BlobContent1"u8.ToArray();
        _blobStorageUnderTest.Add(BUCKET_NAME, blobName, blobContent);

        blobContent = "BlobContent2"u8.ToArray();
        _blobStorageUnderTest.Add(BUCKET_NAME, blobName, blobContent);

        var acting = _blobStorageUnderTest.SaveAsync;
        await acting.Should().ThrowAsync<BlobAlreadyExistsException>();
    }

    [Fact(Skip = "No valid emulator for GCP")]
    public async Task DeleteBlobThatExists()
    {
        _blobStorageUnderTest.Add(BUCKET_NAME, "BlobName", "BlobContent".GetBytes());
        await _blobStorageUnderTest.SaveAsync();

        _blobStorageUnderTest.Remove(BUCKET_NAME, "BlobName");
        await _blobStorageUnderTest.SaveAsync();

        var acting = () => _blobStorageUnderTest.FindAsync(BUCKET_NAME, "BlobName");
        await acting.Should().ThrowAsync<NotFoundException>();
    }

    [Fact(Skip = "No valid emulator for GCP")]
    public async Task DeleteBlobThatDoesNotExist()
    {
        _blobStorageUnderTest.Remove(BUCKET_NAME, "BlobNameThatDoesNotExist");

        var acting = () => _blobStorageUnderTest.FindAsync(BUCKET_NAME, "BlobNameThatDoesNotExist");
        await acting.Should().ThrowAsync<NotFoundException>();
    }

    [Fact(Skip = "No valid emulator for GCP")]
    public async Task SaveAndFindAllBlobs()
    {
        var blob1Content = "BlobContent1".GetBytes();
        var blob2Content = "BlobContent2".GetBytes();

        _blobStorageUnderTest.Add(BUCKET_NAME, "BlobName1", blob1Content);
        _blobStorageUnderTest.Add(BUCKET_NAME, "BlobName2", blob2Content);

        await _blobStorageUnderTest.SaveAsync();

        var retrievedBlobContent = await (await _blobStorageUnderTest.FindAllAsync(BUCKET_NAME)).ToListAsync();

        retrievedBlobContent.Should().Contain("BlobName1");
        retrievedBlobContent.Should().Contain("BlobName2");
    }
}
