using System.Linq.Expressions;
using Backbone.Modules.Announcements.Module.Features.Announcements.DeleteAnnouncementRecipients;
using Backbone.Modules.Announcements.Abstractions;
using Backbone.Modules.Announcements.Domain.Entities;
using FakeItEasy;

namespace Backbone.Modules.Announcements.Module.Tests.Tests.Announcements.Commands.AnonymizeRecipient;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task DeleteRecipientForIdentityCommand_AnonymizesRecipientsSuccessfully()
    {
        // Arrange
        var mockRepository = A.Fake<IAnnouncementsRepository>();

        var handler = new Handler(mockRepository);
        var command = new DeleteAnnouncementRecipientsCommand { IdentityAddress = CreateRandomIdentityAddress().Value };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => mockRepository.DeleteRecipients(A<Expression<Func<AnnouncementRecipient, bool>>>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }
}
