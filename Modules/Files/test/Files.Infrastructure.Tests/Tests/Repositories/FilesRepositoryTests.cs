using System.Data.Common;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Files.Application.Infrastructure.Persistence;
using Backbone.Modules.Files.Domain.Entities;
using Backbone.Modules.Files.Infrastructure.Persistence.Database;
using Backbone.Modules.Files.Infrastructure.Persistence.Database.Repository;
using Backbone.UnitTestTools.Extensions;
using Backbone.UnitTestTools.TestDoubles.Fakes;
using FakeItEasy;
using Microsoft.EntityFrameworkCore.Diagnostics;
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

    [Fact]
    public async Task Deletes_orphaned_blobs_using_batched_database_queries()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var fakeBlobStorage = A.Fake<IBlobStorage>();
        var existingFile1 = GenerateFile(CreateRandomIdentityAddress());
        var existingFile2 = GenerateFile(CreateRandomIdentityAddress());
        var blobIds = Enumerable.Range(0, 1001).Select(_ => FileId.New().Value).ToList();
        blobIds[0] = existingFile1.Id;
        blobIds[1000] = existingFile2.Id;

        A.CallTo(() => fakeBlobStorage.ListAsync(A<string>._, A<string?>._)).Returns(Task.FromResult(AsAsyncEnumerable(blobIds)));

        var queryCounter = new QueryCounterInterceptor();

        var (arrangeContext, actContext, _) = FakeDbContextFactory.CreateDbContexts<FilesDbContext>(interceptors: [queryCounter]);

        await arrangeContext.SaveEntities(existingFile1, existingFile2);

        queryCounter.Reset();

        var repository = new FilesRepository(actContext, fakeBlobStorage, Options.Create(new BlobConfiguration { RootFolder = "" }));

        // Act
        var numberOfDeletedBlobs = await repository.DeleteOrphanedBlobs(cancellationToken);

        // Assert
        numberOfDeletedBlobs.ShouldBe(999);
        queryCounter.NumberOfQueries.ShouldBe(2);
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

    private static async IAsyncEnumerable<string> AsAsyncEnumerable(IEnumerable<string> values)
    {
        await Task.Yield();

        foreach (var value in values)
            yield return value;
    }

    private sealed class QueryCounterInterceptor : DbCommandInterceptor
    {
        public int NumberOfQueries { get; private set; }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            NumberOfQueries++;
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        public void Reset()
        {
            NumberOfQueries = 0;
        }
    }
}
