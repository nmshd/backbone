using Backbone.BuildingBlocks.Application.Identities;
using Backbone.Modules.Tokens.Application.Identities;
using Backbone.Modules.Tokens.Application.Tokens.Commands.DeleteTokensOfIdentity;
using Backbone.UnitTestTools.BaseClasses;
using FakeItEasy;
using MediatR;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Tokens.Application.Tests.Tests.Identities;

public class IdentityDeleterTests : AbstractTestsBase
{
    [Fact]
    public async Task Deleter_calls_correct_command()
    {
        // Arrange
        var mockMediator = A.Fake<IMediator>();
        var dummyIDeletionProcessLogger = A.Dummy<IDeletionProcessLogger>();
        var deleter = new IdentityDeleter(mockMediator, dummyIDeletionProcessLogger);
        var identityAddress = CreateRandomIdentityAddress();

        // Act
        await deleter.Delete(identityAddress);

        // Assert
        A.CallTo(() => mockMediator.Send(A<DeleteTokensOfIdentityCommand>.That.Matches(command => command.IdentityAddress == identityAddress), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task Deleter_correctly_creates_audit_log()
    {
        // Arrange
        var dummyMediator = A.Dummy<IMediator>();
        var mockIDeletionProcessLogger = A.Fake<IDeletionProcessLogger>();
        var deleter = new IdentityDeleter(dummyMediator, mockIDeletionProcessLogger);
        var identityAddress = CreateRandomIdentityAddress();

        // Act
        await deleter.Delete(identityAddress);

        // Assert
        A.CallTo(() => mockIDeletionProcessLogger.LogDeletion(identityAddress, "Tokens")).MustHaveHappenedOnceExactly();
    }
}
