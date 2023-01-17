using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System.Text;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.BlobStorage.GoogleCloudStorage;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Enmeshed.BuildingBlocks.Infrastructure.Tests.Tests
{
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
            _blobStorageUnderTest = new GoogleCloudStorage(BUCKET_NAME, _storageClient, logger);
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

            _blobStorageUnderTest.Add(blobName, blobContent);
            await _blobStorageUnderTest.SaveAsync();

            var retrievedBlobContent = await _blobStorageUnderTest.FindAsync(blobName);
            retrievedBlobContent.Should().Equal(blobContent);
        }

        [Fact(Skip = "No valid emulator for GCP")]
        public async Task SaveAndFindMultipleBlobs()
        {
            var blob1Content = "BlobContent1".GetBytes();
            var blob2Content = "BlobContent2".GetBytes();

            _blobStorageUnderTest.Add("BlobName1", blob1Content);
            _blobStorageUnderTest.Add("BlobName2", blob2Content);

            await _blobStorageUnderTest.SaveAsync();

            var retrievedBlob1Content = await _blobStorageUnderTest.FindAsync("BlobName1");
            var retrievedBlob2Content = await _blobStorageUnderTest.FindAsync("BlobName2");

            retrievedBlob1Content.Should().Equal(blob1Content);
            retrievedBlob2Content.Should().Equal(blob2Content);
        }

        [Fact(Skip = "No valid emulator for GCP")]
        public async Task AddBlobWithSameName()
        {
            const string blobName = "AddBlobWithSameName";

            var blobContent = Encoding.ASCII.GetBytes("BlobContent1");
            _blobStorageUnderTest.Add(blobName, blobContent);

            blobContent = Encoding.ASCII.GetBytes("BlobContent2");
            _blobStorageUnderTest.Add(blobName, blobContent);

            var acting = () => _blobStorageUnderTest.SaveAsync();
            await acting.Should().ThrowAsync<BlobAlreadyExistsException>();
        }

        [Fact(Skip = "No valid emulator for GCP")]
        public async Task DeleteBlobThatExists()
        {
            _blobStorageUnderTest.Add("BlobName", "BlobContent".GetBytes());
            await _blobStorageUnderTest.SaveAsync();

            _blobStorageUnderTest.Remove("BlobName");
            await _blobStorageUnderTest.SaveAsync();

            var acting = () => _blobStorageUnderTest.FindAsync("BlobName");
            await acting.Should().ThrowAsync<NotFoundException>();
        }

        [Fact(Skip = "No valid emulator for GCP")]
        public async Task DeleteBlobThatDoesNotExist()
        {
            _blobStorageUnderTest.Remove("BlobNameThatDoesNotExist");

            var acting = () => _blobStorageUnderTest.FindAsync("BlobNameThatDoesNotExist");
            await acting.Should().ThrowAsync<NotFoundException>();
        }

        [Fact(Skip = "No valid emulator for GCP")]
        public async Task SaveAndFindAllBlobs()
        {
            var blob1Content = "BlobContent1".GetBytes();
            var blob2Content = "BlobContent2".GetBytes();

            _blobStorageUnderTest.Add("BlobName1", blob1Content);
            _blobStorageUnderTest.Add("BlobName2", blob2Content);

            await _blobStorageUnderTest.SaveAsync();

            var retrievedBlobContent = await _blobStorageUnderTest.FindAllAsync();

            Assert.Contains<string>("BlobName1", retrievedBlobContent);
            Assert.Contains<string>("BlobName2", retrievedBlobContent);
        }
    }
}