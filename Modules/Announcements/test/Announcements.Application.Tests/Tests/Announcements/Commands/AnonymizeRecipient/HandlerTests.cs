using System.Linq.Expressions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Announcements.Application.Announcements.Commands.AnonymizeRecipient;
using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Announcements.Domain.Entities;
using FakeItEasy;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Announcements.Application.Tests.Tests.Announcements.Commands.AnonymizeRecipient;

public class HandlerTests : AbstractTestsBase
{
    private const string DID_DOMAIN_NAME = "localhost";

    [Fact]
    public async Task AnonymizeRecipientForIdentityCommand_AnonymizesRecipientsSuccessfully()
    {
        // Arrange
        var mockRepository = A.Fake<IAnnouncementsRepository>();
        var mockOptions = A.Fake<IOptions<ApplicationOptions>>();

        A.CallTo(() => mockOptions.Value).Returns(new ApplicationOptions { DidDomainName = DID_DOMAIN_NAME });

        var handler = new Handler(mockRepository, mockOptions);
        var command = new AnonymizeRecipientForIdentityCommand(CreateRandomIdentityAddress().Value);

        var announcementRecipient = new AnnouncementRecipient(CreateRandomIdentityAddress().Value);
        var announcements = new List<Announcement>
        {
            new(AnnouncementSeverity.Low, [], null, [announcementRecipient])
        };

        A.CallTo(() => mockRepository.FindAllForIdentityAddress(A<Expression<Func<Announcement, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(announcements);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        announcementRecipient.Address.Should().Be(IdentityAddress.GetAnonymized(DID_DOMAIN_NAME));
        A.CallTo(() => mockRepository.FindAllForIdentityAddress(A<Expression<Func<Announcement, bool>>>.Ignored, A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        A.CallTo(() => mockRepository.Update(announcements, A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
    }
}
