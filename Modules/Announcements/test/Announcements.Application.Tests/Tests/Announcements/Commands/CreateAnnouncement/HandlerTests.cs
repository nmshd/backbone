using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Announcements.Application.Announcements.Commands.CreateAnnouncement;
using Backbone.Modules.Announcements.Application.Announcements.DTOs;
using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Announcements.Domain.Entities;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Announcements.Application.Tests.Tests.Announcements.Commands.CreateAnnouncement;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Handle_WhenRecipientsAreNull_ShouldNotCallIdentityRepository()
    {
        // Arrange
        var mockAnnouncementsRepository = A.Fake<IAnnouncementsRepository>();
        var mockLogger = A.Fake<ILogger<Handler>>();
        var mockIdentityRepository = A.Fake<IIdentitiesRepository>();
        var handler = new Handler(mockLogger, mockAnnouncementsRepository, mockIdentityRepository);
        var command = new CreateAnnouncementCommand
        {
            Recipients = null,
            Severity = AnnouncementSeverity.Low,
            Texts = []
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => mockIdentityRepository.Find(
                A<Expression<Func<Identity, bool>>>.Ignored,
                A<CancellationToken>.Ignored,
                A<bool>.Ignored))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenRecipientsAreEmpty_ShouldNotCallIdentityRepository()
    {
        // Arrange
        var mockAnnouncementsRepository = A.Fake<IAnnouncementsRepository>();
        var mockLogger = A.Fake<ILogger<Handler>>();
        var mockIdentityRepository = A.Fake<IIdentitiesRepository>();
        var handler = new Handler(mockLogger, mockAnnouncementsRepository, mockIdentityRepository);
        var command = new CreateAnnouncementCommand
        {
            Recipients = [],
            Severity = AnnouncementSeverity.Low,
            Texts = []
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => mockIdentityRepository.Find(
                A<Expression<Func<Identity, bool>>>.Ignored,
                A<CancellationToken>.Ignored,
                A<bool>.Ignored))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenRecipientIdsAreInvalid_ShouldThrowException()
    {
        // Arrange
        var mockAnnouncementsRepository = A.Fake<IAnnouncementsRepository>();
        var mockLogger = A.Fake<ILogger<Handler>>();
        var mockIdentityRepository = A.Fake<IIdentitiesRepository>();
        var handler = new Handler(mockLogger, mockAnnouncementsRepository, mockIdentityRepository);
        var command = new CreateAnnouncementCommand
        {
            Recipients = new List<string> { "recipient1", "recipient2" },
            Severity = AnnouncementSeverity.Low,
            Texts = []
        };

        // Act
        Func<Task> act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => mockIdentityRepository.Find(
                A<Expression<Func<Identity, bool>>>.Ignored,
                A<CancellationToken>.Ignored,
                A<bool>.Ignored))
            .MustNotHaveHappened();

        await act.Should().ThrowAsync<InvalidIdException>();
    }

    [Fact]
    public async Task Handle_WhenRecipientsAreValid_ShouldCallIdentityRepository()
    {
        // Arrange
        var mockAnnouncementsRepository = A.Fake<IAnnouncementsRepository>();
        var mockLogger = A.Fake<ILogger<Handler>>();
        var mockIdentityRepository = A.Fake<IIdentitiesRepository>();
        var handler = new Handler(mockLogger, mockAnnouncementsRepository, mockIdentityRepository);
        var command = new CreateAnnouncementCommand
        {
            Recipients = new List<string> { "did:e:localhost:dids:c179861648989f28d189c9", "did:e:localhost:dids:656aa0be80fe83a70aa173" },
            Severity = AnnouncementSeverity.Low,
            Texts = []
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => mockIdentityRepository.Find(
                A<Expression<Func<Identity, bool>>>.Ignored,
                A<CancellationToken>.Ignored,
                A<bool>.Ignored))
            .MustHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenRecipientsAreValid_ShouldCallAnnouncementsRepository()
    {
        // Arrange
        var mockAnnouncementsRepository = A.Fake<IAnnouncementsRepository>();
        var mockLogger = A.Fake<ILogger<Handler>>();
        var mockIdentityRepository = A.Fake<IIdentitiesRepository>();
        var handler = new Handler(mockLogger, mockAnnouncementsRepository, mockIdentityRepository);
        var command = new CreateAnnouncementCommand
        {
            Recipients = new List<string> { "did:e:localhost:dids:c179861648989f28d189c9", "did:e:localhost:dids:656aa0be80fe83a70aa173" },
            Severity = AnnouncementSeverity.Low,
            Texts = []
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => mockAnnouncementsRepository.Add(
                A<Announcement>.Ignored,
                A<CancellationToken>.Ignored))
            .MustHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenRecipientsAreValid_ShouldReturnAnnouncementDTO()
    {
        // Arrange
        var mockAnnouncementsRepository = A.Fake<IAnnouncementsRepository>();
        var mockLogger = A.Fake<ILogger<Handler>>();
        var mockIdentityRepository = A.Fake<IIdentitiesRepository>();
        var handler = new Handler(mockLogger, mockAnnouncementsRepository, mockIdentityRepository);
        var command = new CreateAnnouncementCommand
        {
            Recipients = new List<string> { "did:e:localhost:dids:c179861648989f28d189c9", "did:e:localhost:dids:656aa0be80fe83a70aa173" },
            Severity = AnnouncementSeverity.Low,
            Texts = []
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<AnnouncementDTO>();
    }

    [Fact]
    public async Task Handle_WhenRecipientsAreValid_ShouldReturnAnnouncementDTOWithCorrectRecipients()
    {
        // Arrange
        var mockAnnouncementsRepository = A.Fake<IAnnouncementsRepository>();
        var mockLogger = A.Fake<ILogger<Handler>>();
        var mockIdentityRepository = A.Fake<IIdentitiesRepository>();
        var handler = new Handler(mockLogger, mockAnnouncementsRepository, mockIdentityRepository);

        const string id1 = "did:e:localhost:dids:c179861648989f28d189c9";
        const string id2 = "did:e:localhost:dids:656aa0be80fe83a70aa173";

        A.CallTo(() => mockIdentityRepository.Find(
                A<Expression<Func<Identity, bool>>>.Ignored,
                A<CancellationToken>.Ignored,
                A<bool>.Ignored))
            .Returns(
            [
                new Identity("clientId1", IdentityAddress.Parse(id1), [], TierId.Generate(), 1, CommunicationLanguage.DEFAULT_LANGUAGE),
                new Identity("clientId1", IdentityAddress.Parse(id2), [], TierId.Generate(), 1, CommunicationLanguage.DEFAULT_LANGUAGE),
            ]);

        var command = new CreateAnnouncementCommand
        {
            Recipients = new List<string> { id1, id2 },
            Severity = AnnouncementSeverity.Low,
            Texts = []
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Recipients.Select(r => r.Address).Should().BeEquivalentTo(command.Recipients);
    }

    [Fact]
    public async Task Handle_WhenOneRecipientIsNotFound_ShoulReturnOnlyFoundRecipients()
    {
        // Arrange
        var mockAnnouncementsRepository = A.Fake<IAnnouncementsRepository>();
        var mockLogger = A.Fake<ILogger<Handler>>();
        var mockIdentityRepository = A.Fake<IIdentitiesRepository>();
        var handler = new Handler(mockLogger, mockAnnouncementsRepository, mockIdentityRepository);

        const string id1 = "did:e:localhost:dids:c179861648989f28d189c9";
        const string id2 = "did:e:localhost:dids:656aa0be80fe83a70aa173";

        var command = new CreateAnnouncementCommand
        {
            Recipients = new List<string> { id1, id2 },
            Severity = AnnouncementSeverity.Low,
            Texts = []
        };

        var requestRecipients = command.Recipients.OrderBy(address => address).ToList();
        var notFoundRecipientAddresses = requestRecipients.Except([id2]).ToList();

        A.CallTo(() => mockIdentityRepository.Find(
                A<Expression<Func<Identity, bool>>>.Ignored,
                A<CancellationToken>.Ignored,
                A<bool>.Ignored))
            .Returns(
            [
                new Identity("clientId1", IdentityAddress.Parse(id1), [], TierId.Generate(), 1, CommunicationLanguage.DEFAULT_LANGUAGE),
            ]);


        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();

        result.Recipients.First().Address.Should().Be(id1);
    }
}
