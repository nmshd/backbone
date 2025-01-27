using System.Linq.Expressions;
using Backbone.Modules.Announcements.Application.Announcements.Commands.DeleteAnnouncementRecipients;
using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Announcements.Domain.Entities;
using FakeItEasy;

namespace Backbone.Modules.Announcements.Application.Tests.Tests.Announcements.Commands.AnonymizeRecipient;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task AnonymizeRecipientForIdentityCommand_AnonymizesRecipientsSuccessfully()
    {
        // Arrange
        var mockRepository = A.Fake<IAnnouncementsRepository>();

        var handler = new Handler(mockRepository);
        var command = new DeleteAnnouncementRecipientsCommand(CreateRandomIdentityAddress().Value);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => mockRepository.DeleteRecipients(A<Expression<Func<AnnouncementRecipient, bool>>>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }
}
