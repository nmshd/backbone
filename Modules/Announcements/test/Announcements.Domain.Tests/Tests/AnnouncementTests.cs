using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Announcements.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Announcements.Domain.Entities;
using Backbone.Tooling;
using Backbone.UnitTestTools.Extensions;

namespace Backbone.Modules.Announcements.Domain.Tests.Tests;

public class AnnouncementTests : AbstractTestsBase
{
    [Fact]
    public void Raises_AnnouncementCreatedDomainEvent()
    {
        // Arrange
        var utcNow = DateTime.UtcNow;
        SystemTime.Set(utcNow);

        // Act
        var announcement = new Announcement(
            AnnouncementSeverity.High,
            true,
            [new AnnouncementText(AnnouncementLanguage.DEFAULT_LANGUAGE, "Test Title", "Test Body")],
            expiresAt: null,
            recipients: [new AnnouncementRecipient(CreateRandomIdentityAddress())],
            iqlQuery: AnnouncementIqlQuery.Parse("StreetAddress.city='Heidelberg' && #'Primary Address'")
        );

        // Assert
        var domainEvent = announcement.Should().HaveASingleDomainEvent<AnnouncementCreatedDomainEvent>();
        domainEvent.Id.Should().Be(announcement.Id);
        domainEvent.Severity.Should().Be("High");
        domainEvent.Texts.Should().HaveCount(1);
        domainEvent.Recipients.Should().HaveCount(1);
        domainEvent.CreationDate.Should().Be(utcNow);
    }

    [Fact]
    public void Cannot_have_an_iql_query_when_the_announcement_is_not_silent()
    {
        // Act
        var acting = () => new Announcement(
            AnnouncementSeverity.High,
            false,
            [new AnnouncementText(AnnouncementLanguage.DEFAULT_LANGUAGE, "Test Title", "Test Body")],
            expiresAt: null,
            recipients: [new AnnouncementRecipient(CreateRandomIdentityAddress())],
            iqlQuery: AnnouncementIqlQuery.Parse("StreetAddress.city='Heidelberg' && #'Primary Address'")
        );

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.nonSilentAnnouncementCannotHaveIqlQuery");
    }
}
