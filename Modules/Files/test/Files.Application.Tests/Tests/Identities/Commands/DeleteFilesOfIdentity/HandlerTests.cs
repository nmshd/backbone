using System.Linq.Expressions;
using Backbone.Modules.Files.Application.Identities.Commands.DeleteFilesOfIdentity;
using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using FakeItEasy;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Application.Tests.Tests.Identities.Commands.DeleteFilesOfIdentity;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Handler_calls_deletion_method_on_repository()
    {
        // Arrange
        var mockFilesRepository = A.Fake<IFilesRepository>();
        var handler = CreateHandler(mockFilesRepository);
        var identityAddress = CreateRandomIdentityAddress();

        // Act
        await handler.Handle(new DeleteFilesOfIdentityCommand { IdentityAddress = identityAddress }, CancellationToken.None);

        // Assert
        A.CallTo(() => mockFilesRepository.DeleteFilesOfIdentity(A<Expression<Func<File, bool>>>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    private static Handler CreateHandler(IFilesRepository? filesRepository = null)
    {
        return new Handler(filesRepository ?? A.Fake<IFilesRepository>());
    }
}
