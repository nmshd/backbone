using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Announcements.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Announcements.Domain.Entities;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;

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
        var domainEvent = announcement.ShouldHaveASingleDomainEvent<AnnouncementCreatedDomainEvent>();
        domainEvent.Id.ShouldBe(announcement.Id);
        domainEvent.Severity.ShouldBe("High");
        domainEvent.Texts.ShouldHaveCount(1);
        domainEvent.Recipients.ShouldHaveCount(1);
        domainEvent.CreationDate.ShouldBe(utcNow);
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
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.nonSilentAnnouncementCannotHaveIqlQuery");
    }
}
