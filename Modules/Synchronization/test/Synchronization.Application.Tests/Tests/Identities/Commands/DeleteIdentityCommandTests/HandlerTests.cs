using Backbone.Modules.Synchronization.Application.Identities.Commands.DeleteIdentity;
using FakeItEasy;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.Identities.Commands.DeleteIdentityCommandTests;
public class HandlerTests
{
    [Fact]
    public async Task TODO()
    {
        // Arrange
        //var handler = CreateHandler();

        // Act
        //await handler.Handle(new DeleteIdentityCommand("identity-address"), CancellationToken.None);
        // cannot mock dbcontext trivially
        // Assert

    }

    private static Handler CreateHandler()
    {
        return A.Fake<Handler>();
    }
}
