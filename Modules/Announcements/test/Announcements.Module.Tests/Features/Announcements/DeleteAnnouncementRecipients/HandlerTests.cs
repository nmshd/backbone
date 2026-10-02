using System.Linq.Expressions;
using Backbone.Modules.Announcements.Abstractions;
using Backbone.Modules.Announcements.Domain.Entities;
using Backbone.Modules.Announcements.Module.Features.Announcements.DeleteAnnouncementRecipients;
using FakeItEasy;
using DeleteAnnouncementRecipientsSlice = Backbone.Modules.Announcements.Module.Features.Announcements.DeleteAnnouncementRecipients;

namespace Backbone.Modules.Announcements.Module.Tests.Features.Announcements.DeleteAnnouncementRecipients;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task DeleteRecipientForIdentityCommand_AnonymizesRecipientsSuccessfully()
    {
        // Arrange
        var mockRepository = A.Fake<IAnnouncementsRepository>();

        var handler = new Handler(mockRepository);
        var command = new DeleteAnnouncementRecipientsSlice.Command { IdentityAddress = CreateRandomIdentityAddress().Value };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => mockRepository.DeleteRecipients(A<Expression<Func<AnnouncementRecipient, bool>>>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }
}
