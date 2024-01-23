using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Files.Application.Infrastructure.Persistence;
using Backbone.Modules.Files.Infrastructure.Persistence.Database;
using Backbone.Modules.Files.Infrastructure.Persistence.Database.Repository;
using Backbone.UnitTestTools.Data;
using FakeItEasy;
using Microsoft.Extensions.Options;
using MockQueryable.FakeItEasy;
using Xunit;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Infrastructure.Tests.Tests.Repositories;

public class FilesRepositoryTests
{
    [Fact]
    public async Task Calls_BlobStorage_Remove_with_correct_FileIds()
    {
        // Arrange
        var mockBlobStorage = A.Fake<IBlobStorage>();
        var fakeDbContext = A.Fake<FilesDbContext>();
        var blobStorageOptions = Options.Create(new BlobOptions());

        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();

        var files = new List<File> { GenerateFile(identityAddress), GenerateFile(identityAddress) };
        fakeDbContext.FileMetadata = files.AsQueryable().BuildMockDbSet();

        var repository = new FilesRepository(fakeDbContext, mockBlobStorage, blobStorageOptions);

        // Act
        await repository.DeleteFilesOfIdentity(identityAddress, CancellationToken.None);

        // Assert
        foreach (var file in files)
        {
            A.CallTo(() => mockBlobStorage.Remove(A<string>._, file.Id)).MustHaveHappenedOnceExactly();
        }
    }

    private static File GenerateFile(IdentityAddress identityAddress)
    {
        return new File(identityAddress, TestDataGenerator.CreateRandomDeviceId(), identityAddress, [], [], [], 0, DateTime.Now, []);
    }
}
