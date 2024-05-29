using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Files.Application.Infrastructure.Persistence;
using Backbone.Modules.Files.Infrastructure.Persistence.Database;
using Backbone.Modules.Files.Infrastructure.Persistence.Database.Repository;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using FakeItEasy;
using Microsoft.Extensions.Options;
using MockQueryable.FakeItEasy;
using Xunit;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Infrastructure.Tests.Tests.Repositories;

public class FilesRepositoryTests : AbstractTestsBase
{
    [Fact]
    public async Task Calls_BlobStorage_Remove_with_correct_FileIds()
    {
        // Arrange
        var mockBlobStorage = A.Fake<IBlobStorage>();

        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var files = new List<File> { GenerateFile(identityAddress), GenerateFile(identityAddress) };
        var repository = CreateFilesRepository(files, mockBlobStorage);

        // Act
        await repository.DeleteFilesOfIdentity(File.WasCreatedBy(identityAddress), CancellationToken.None);

        // Assert
        A.CallTo(() => mockBlobStorage.Remove(A<string>._, A<string>.That.Matches(fileId => files.Any(f => f.Id == fileId)))).MustHaveHappenedANumberOfTimesMatching(x => x == files.Count);
    }

    private static File GenerateFile(IdentityAddress identityAddress)
    {
        return new File(identityAddress, TestDataGenerator.CreateRandomDeviceId(), identityAddress, [], [], [], 0, DateTime.Now, []);
    }

    private static FilesRepository CreateFilesRepository(List<File> files, IBlobStorage mockBlobStorage)
    {
        var fakeDbContext = A.Fake<FilesDbContext>();
        var blobStorageOptions = Options.Create(new BlobOptions() { RootFolder = "" });

        fakeDbContext.FileMetadata = files.AsQueryable().BuildMockDbSet();

        return new FilesRepository(fakeDbContext, mockBlobStorage, blobStorageOptions);
    }
}
