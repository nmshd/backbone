using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Files.Application.Infrastructure.Persistence;
using Backbone.Modules.Files.Infrastructure.Persistence.Database;
using Backbone.Modules.Files.Infrastructure.Persistence.Database.Repository;
using Backbone.UnitTestTools.TestDoubles.Fakes;
using FakeItEasy;
using Microsoft.Extensions.Options;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Infrastructure.Tests.Tests.Repositories;

public class FilesRepositoryTests : AbstractTestsBase
{
    [Fact]
    public async Task Calls_BlobStorage_Remove_with_correct_FileIds()
    {
        // Arrange
        var mockBlobStorage = A.Fake<IBlobStorage>();

        var identityAddress = CreateRandomIdentityAddress();
        var files = new List<File> { GenerateFile(identityAddress), GenerateFile(identityAddress) };
        var repository = CreateFilesRepository(files, mockBlobStorage);

        // Act
        await repository.DeleteFilesOfIdentity(File.IsOwnedBy(identityAddress), CancellationToken.None);

        // Assert
        A.CallTo(() => mockBlobStorage.Remove(A<string>._, A<string>.That.Matches(fileId => files.Any(f => f.Id == fileId)))).MustHaveHappenedANumberOfTimesMatching(x => x == files.Count);
    }

    private static File GenerateFile(IdentityAddress identityAddress)
    {
        return new File(identityAddress, CreateRandomDeviceId(), [], [], [], 0, DateTime.Now, []);
    }

    private static FilesRepository CreateFilesRepository(List<File> files, IBlobStorage mockBlobStorage)
    {
        var blobStorageOptions = Options.Create(new BlobConfiguration { RootFolder = "" });

        var (arrangeContext, actContext, _) = FakeDbContextFactory.CreateDbContexts<FilesDbContext>();

        arrangeContext.AddRange(files);
        arrangeContext.SaveChanges();

        return new FilesRepository(actContext, mockBlobStorage, blobStorageOptions);
    }
}
