using Backbone.BuildingBlocks.Application.Identities;
using Backbone.Modules.Announcements.Application.Announcements;
using Backbone.Modules.Announcements.Application.Announcements.Commands.DeleteAnnouncementRecipients;
using FakeItEasy;
using MediatR;

namespace Backbone.Modules.Announcements.Application.Tests.Tests.Announcements;

public class IdentityDeleterTests : AbstractTestsBase
{
    [Fact]
    public async Task Deleter_calls_correct_command()
    {
        // Arrange
        var mockMediator = A.Fake<IMediator>();
        var mockIDeletionProcessLogger = A.Dummy<IDeletionProcessLogger>();
        var deleter = new IdentityDeleter(mockMediator, mockIDeletionProcessLogger);
        var identityAddress = CreateRandomIdentityAddress();

        // Act
        await deleter.Delete(identityAddress);

        // Assert
        A.CallTo(() => mockMediator.Send(A<DeleteAnnouncementRecipientsCommand>
                .That
                .Matches(command => command.IdentityAddress == identityAddress), A<CancellationToken>._))
            .MustHaveHappened();
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
        A.CallTo(() => mockIDeletionProcessLogger.LogDeletion(identityAddress, "AnnouncementRecipients")).MustHaveHappenedOnceExactly();
    }
}
