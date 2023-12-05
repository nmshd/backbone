using Backbone.Modules.Challenges.Application.Identities;
using Backbone.Modules.Challenges.Application.Identities.Commands.DeleteIdentity;
using Backbone.UnitTestTools.Data;
using FakeItEasy;
using MediatR;
using Xunit;

namespace Backbone.Modules.Challenges.Application.Tests.Tests.Identities;
public class IdentityDeleterTests
{
    [Fact]
    public async Task Deleter_calls_correct_command()
    {
        // Arrange
        var mediator = A.Fake<IMediator>();
        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var deleter = new IdentityDeleter(mediator);

        // Act
        await deleter.Delete(identityAddress);

        // Assert
        A.CallTo(() => mediator.Send(A<DeleteIdentityCommand>.That.Matches(it => it.IdentityAddress == identityAddress), A<CancellationToken>._)).MustHaveHappened();
    }
}
