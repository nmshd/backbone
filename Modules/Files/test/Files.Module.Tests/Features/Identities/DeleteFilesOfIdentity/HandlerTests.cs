using DeleteFilesOfIdentitySlice = Backbone.Modules.Files.Module.Features.Identities.DeleteFilesOfIdentity;
using System.Linq.Expressions;
using Backbone.Modules.Files.Module.Features.Identities.DeleteFilesOfIdentity;
using Backbone.Modules.Files.Abstractions;
using FakeItEasy;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Module.Tests.Features.Identities.DeleteFilesOfIdentity;

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
        await handler.Handle(new DeleteFilesOfIdentitySlice.Command { IdentityAddress = identityAddress }, CancellationToken.None);

        // Assert
        A.CallTo(() => mockFilesRepository.DeleteFilesOfIdentity(A<Expression<Func<File, bool>>>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    private static Handler CreateHandler(IFilesRepository? filesRepository = null)
    {
        return new Handler(filesRepository ?? A.Fake<IFilesRepository>());
    }
}
