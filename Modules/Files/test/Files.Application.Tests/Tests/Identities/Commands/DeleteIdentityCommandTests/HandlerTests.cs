using Backbone.Modules.Files.Application.Identities.Commands.DeleteIdentity;
using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using FakeItEasy;
using Xunit;

namespace Backbone.Modules.Files.Application.Tests.Tests.Identities.Commands.DeleteIdentityCommandTests;
public class HandlerTests
{
    [Fact]
    public async Task Handler_calls_deletion_method_on_repository()
    {
        // Arrange
        var identityAddress = "identity-address";
        var challengesRepository = A.Fake<IFilesRepository>();
        var handler = CreateHandler(challengesRepository);

        // Act
        await handler.Handle(new DeleteIdentityCommand(identityAddress), CancellationToken.None);

        // Assert
        A.CallTo(() => challengesRepository.DeleteFilesByCreator(identityAddress, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    private static Handler CreateHandler(IFilesRepository challengesRepository = null)
    {
        return new Handler(challengesRepository ?? A.Fake<IFilesRepository>());
    }
}
