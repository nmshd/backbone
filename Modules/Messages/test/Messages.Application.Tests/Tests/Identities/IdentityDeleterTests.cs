using Backbone.BuildingBlocks.Application.Identities;
using Backbone.Modules.Messages.Application.Identities;
using Backbone.Modules.Messages.Application.Messages.Commands.AnonymizeMessagesOfIdentity;
using Backbone.UnitTestTools.BaseClasses;
using FakeItEasy;
using MediatR;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Messages.Application.Tests.Tests.Identities;

public class IdentityDeleterTests : AbstractTestsBase
{
    [Fact]
    public async Task Deleter_calls_correct_command()
    {
        // Arrange
        var mockMediator = A.Fake<IMediator>();
        var deleter = new IdentityDeleter(mockMediator);
        var identityAddress = CreateRandomIdentityAddress();
        var dummyIDeletionProcessLogger = A.Fake<IDeletionProcessLogger>();

        // Act
        await deleter.Delete(identityAddress, dummyIDeletionProcessLogger);

        // Assert
        A.CallTo(() => mockMediator.Send(A<AnonymizeMessagesOfIdentityCommand>.That.Matches(command => command.IdentityAddress == identityAddress), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task Deleter_correctly_creates_audit_log()
    {
        // Arrange
        var dummyMediator = A.Fake<IMediator>();
        var deleter = new IdentityDeleter(dummyMediator);
        var identityAddress = CreateRandomIdentityAddress();
        var mockIDeletionProcessLogger = A.Fake<IDeletionProcessLogger>();

        // Act
        await deleter.Delete(identityAddress, mockIDeletionProcessLogger);

        // Assert
        A.CallTo(() => mockIDeletionProcessLogger.LogDeletion(identityAddress, "Messages")).MustHaveHappenedOnceExactly();
    }
}
