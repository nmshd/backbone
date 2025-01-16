using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Announcements.Application.Announcements.Commands.AnonymizeRecipient;
using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Announcements.Domain.Entities;
using FakeItEasy;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Announcements.Application.Tests.Tests.Announcements.Commands.AnonymizeRecipient;

public class HandlerTests : AbstractTestsBase
{
    private const string IDENTITY_ADDRESS1 = "did:e:localhost:dids:c179861648989f28d189c9";

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_WhenRecipientIsNull_ShouldReturnImmediately(string? identityAddress)
    {
        // Arrange
        var mockAnnouncementRecipientRepository = A.Fake<IAnnouncementRecipientRepository>();
        var mockApplicationOptions = A.Fake<IOptions<ApplicationOptions>>();
        var handler = new Handler(mockAnnouncementRecipientRepository, mockApplicationOptions);
        var command = new AnonymizeRecipientForIdentityCommand(identityAddress!);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => mockAnnouncementRecipientRepository.FindAllForIdentityAddress(
                A<IdentityAddress>.Ignored,
                A<CancellationToken>.Ignored))
            .MustNotHaveHappened();

        A.CallTo(() => mockAnnouncementRecipientRepository.Update(
            A<List<AnnouncementRecipient>>.Ignored,
            A<CancellationToken>.Ignored)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenRecipientIsNotNullEmptyOrWhitespace_ShouldAnonymizeRecipient()
    {
        // Arrange
        var mockAnnouncementRecipientRepository = A.Fake<IAnnouncementRecipientRepository>();
        var mockApplicationOptions = A.Fake<IOptions<ApplicationOptions>>();
        A.CallTo(() => mockApplicationOptions.Value).Returns(
            new ApplicationOptions
            {
                DidDomainName = "localhost",
                Pagination = new PaginationOptions()
            });


        var handler = new Handler(mockAnnouncementRecipientRepository, mockApplicationOptions);
        var command = new AnonymizeRecipientForIdentityCommand(IDENTITY_ADDRESS1);

        var identityAddress = IdentityAddress.Parse(IDENTITY_ADDRESS1);
        var cancellationToken = CancellationToken.None;

        var expectedAnnouncementRecipient = new AnnouncementRecipient(identityAddress);
        expectedAnnouncementRecipient.Anonymize(mockApplicationOptions.Value.DidDomainName);


        var announcementRecipient = new AnnouncementRecipient(identityAddress);
        var announcementRecipients = new List<AnnouncementRecipient>
        {
            announcementRecipient
        };

        A.CallTo(() => mockAnnouncementRecipientRepository.FindAllForIdentityAddress(identityAddress, cancellationToken))
            .Returns(announcementRecipients);


        // Act
        await handler.Handle(command, cancellationToken);

        // Assert
        A.CallTo(() => mockAnnouncementRecipientRepository.FindAllForIdentityAddress(identityAddress, cancellationToken))
            .MustHaveHappenedOnceExactly();


        A.CallTo(() => mockAnnouncementRecipientRepository.Update(announcementRecipients, cancellationToken))
            .MustHaveHappenedOnceExactly();

        announcementRecipient.Address.Should().Be(expectedAnnouncementRecipient.Address);
    }

    [Fact]
    public async Task Handle_WhenRecipientsAreNotNullEmptyOrWhitespace_ShouldAnonymizeRecipients()
    {
        // Arrange
        var mockAnnouncementRecipientRepository = A.Fake<IAnnouncementRecipientRepository>();
        var mockApplicationOptions = A.Fake<IOptions<ApplicationOptions>>();
        A.CallTo(() => mockApplicationOptions.Value).Returns(
            new ApplicationOptions
            {
                DidDomainName = "localhost",
                Pagination = new PaginationOptions()
            });


        var handler = new Handler(mockAnnouncementRecipientRepository, mockApplicationOptions);
        var command = new AnonymizeRecipientForIdentityCommand(IDENTITY_ADDRESS1);

        var identityAddress = IdentityAddress.Parse(IDENTITY_ADDRESS1);
        var cancellationToken = CancellationToken.None;

        var expectedAnnouncementRecipients = new List<AnnouncementRecipient>
        {
            new(identityAddress) { AnnouncementId = AnnouncementId.Parse("1") },
            new(identityAddress) { AnnouncementId = AnnouncementId.Parse("1") }
        };

        foreach (var expectedAnnouncementRecipient in expectedAnnouncementRecipients)
        {
            expectedAnnouncementRecipient.Anonymize(mockApplicationOptions.Value.DidDomainName);
        }

        var announcementRecipient1 = new AnnouncementRecipient(identityAddress) { AnnouncementId = AnnouncementId.Parse("1") };
        var announcementRecipient2 = new AnnouncementRecipient(identityAddress) { AnnouncementId = AnnouncementId.Parse("2") };

        var announcementRecipients = new List<AnnouncementRecipient>
        {
            announcementRecipient1,
            announcementRecipient2
        };

        A.CallTo(() => mockAnnouncementRecipientRepository.FindAllForIdentityAddress(identityAddress, cancellationToken))
            .Returns(announcementRecipients);


        // Act
        await handler.Handle(command, cancellationToken);

        // Assert
        A.CallTo(() => mockAnnouncementRecipientRepository.FindAllForIdentityAddress(identityAddress, cancellationToken))
            .MustHaveHappenedOnceExactly();


        A.CallTo(() => mockAnnouncementRecipientRepository.Update(announcementRecipients, cancellationToken))
            .MustHaveHappenedOnceExactly();

        announcementRecipients.Select(a => a.Address).ToArray()
            .Should()
            .BeEquivalentTo(expectedAnnouncementRecipients.Select(e => e.Address).ToArray());
    }
}
