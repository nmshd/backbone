using Backbone.Modules.Files.Application.Identities.Commands.DeleteIdentity;
using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using FakeItEasy;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Files.Application.Tests.Tests.Identities.Commands.DeleteIdentityCommandTests;
public class HandlerTests
{
    [Fact]
    public async Task Handler_calls_deletion_method_on_repository()
    {
        // Arrange
        var filesRepository = A.Fake<IFilesRepository>();
        var handler = CreateHandler(filesRepository);
        var identityAddress = CreateRandomIdentityAddress();

        // Act
        await handler.Handle(new DeleteIdentityCommand(identityAddress), CancellationToken.None);

        // Assert
        A.CallTo(() => filesRepository.DeleteFilesByCreator(identityAddress, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    private static Handler CreateHandler(IFilesRepository filesRepository = null)
    {
        return new Handler(filesRepository ?? A.Fake<IFilesRepository>());
    }
}
