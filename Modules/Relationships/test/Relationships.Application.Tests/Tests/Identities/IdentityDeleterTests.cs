using Backbone.Modules.Relationships.Application.Identities;
using Backbone.Modules.Relationships.Application.Identities.Commands.DeleteRelationshipCommand;
using FakeItEasy;
using MediatR;
using Xunit;

namespace Backbone.Modules.Relationships.Application.Tests.Tests.Identities;
public class IdentityDeleterTests
{
    [Fact]
    public async Task Deleter_calls_correct_command()
    {
        // Arrange
        var identityAddress = TestDataGenerator.CreateRandomAddress();
        var mediator = A.Fake<IMediator>();
        var deleter = new IdentityDeleter(mediator);

        // Act
        await deleter.Delete(identityAddress);

        // Assert
        A.CallTo(() => mediator.Send(A<DeleteRelationshipsCommand>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }
}
