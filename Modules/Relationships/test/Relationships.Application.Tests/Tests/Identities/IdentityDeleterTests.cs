using Backbone.Modules.Relationships.Application.Identities;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.DeleteRelationshipTemplatesOfIdentity;
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
        var identityAddress = UnitTestTools.Data.TestDataGenerator.CreateRandomIdentityAddress();
        var mediator = A.Fake<IMediator>();
        var deleter = new IdentityDeleter(mediator);

        // Act
        await deleter.Delete(identityAddress);

        // Assert
        A.CallTo(() => mediator.Send(A<DeleteRelationshipTemplatesOfIdentityCommand>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }
}
