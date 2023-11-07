using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Devices.Application.Tests;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database.Repository;
using Backbone.Tooling;
using Backbone.UnitTestTools.TestDoubles.Fakes;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Backbone.Modules.Messages.Application.Tests;

public class MessagesRepositoryTest
{
    private readonly MessagesDbContext _arrangeContext;
    private readonly MessagesDbContext _actContext;

    public MessagesRepositoryTest()
    {
        (_arrangeContext, _actContext, _) = FakeDbContextFactory.CreateDbContexts<MessagesDbContext>();
    }

    [Fact]
    public async Task Message_body_can_be_retrieved_from_blob_storage_if_unavailable_in_database()
    {
        // Arrange
        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var deviceId = TestDataGenerator.CreateRandomDeviceId();

        var messageWithFilledBody = new Message(
            identityAddress,
            deviceId,
            SystemTime.UtcNow,
            new byte[] { 1, 2, 3 },
            new Attachment[] { },
            new RecipientInformation[] { });

        var messageWithEmptyBody = new Message(
            identityAddress,
            deviceId,
            SystemTime.UtcNow,
            ""u8.ToArray(),
            new Attachment[] { },
            new RecipientInformation[] { });

        var existingMessages = new List<Message>() {
            messageWithFilledBody,
            messageWithEmptyBody
        };

        await _arrangeContext.Messages.AddRangeAsync(existingMessages);
        await _arrangeContext.SaveChangesAsync();

        var fakeBlobStorage = A.Fake<IBlobStorage>();

        A.CallTo(() => fakeBlobStorage.FindAsync(
                A<string>._, messageWithEmptyBody.Id))
            .Returns(new byte[] { 9, 8, 7 });

        var messageRepository = new MessagesRepository(_actContext, fakeBlobStorage, A.Fake<IOptions<BlobOptions>>());

        // Act
        var messages = (await messageRepository.FindMessagesWithIds(
                new List<MessageId>(),
                identityAddress,
                A.Fake<PaginationFilter>(),
                CancellationToken.None))
            .ItemsOnPage.ToList();

        // Assert
        messages.Count.Should().Be(2);
        messages[0].Body.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
        messages[1].Body.Should().BeEquivalentTo(new byte[] { 9, 8, 7 });
    }
}
