using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.Modules.Files.Application.Infrastructure.Persistence;
using Backbone.Modules.Files.Infrastructure.Persistence.Database;
using Backbone.Modules.Files.Infrastructure.Persistence.Database.Repository;
using FakeItEasy;
using Microsoft.Extensions.Options;
using Xunit;
using Backbone.UnitTestTools.Data;
using File = Backbone.Modules.Files.Domain.Entities.File;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using MockQueryable.FakeItEasy;

namespace Backbone.Modules.Files.Infrastructure.Tests.Tests.Repositories;

public class FilesRepositoryTests
{
    [Fact]
    public async Task Calls_BlobStorage_Remove_with_correct_FileIds()
    {
        // Arrange
        var blobStorage = A.Fake<IBlobStorage>();
        var dbContext = A.Fake<FilesDbContext>();
        var blobStorageOptions = Options.Create(new BlobOptions());

        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();

        var files = new List<File>() { GenerateFile(identityAddress), GenerateFile(identityAddress) };
        dbContext.FileMetadata = files.AsQueryable().BuildMockDbSet(); ;

        var repository = new FilesRepository(dbContext, blobStorage, blobStorageOptions);

        // Act
        await repository.DeleteFilesOfIdentity(identityAddress, CancellationToken.None);

        // Assert
        foreach (var file in files)
        {
            A.CallTo(() => blobStorage.Remove(A<string>._, file.Id)).MustHaveHappenedOnceExactly();
        }
    }

    private File GenerateFile(IdentityAddress identityAddress)
    {
        return new(identityAddress, TestDataGenerator.CreateRandomDeviceId(), identityAddress, [], [], [], 0, DateTime.Now, []);
    }
}
