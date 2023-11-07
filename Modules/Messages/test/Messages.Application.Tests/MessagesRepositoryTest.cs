using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database.Repository;
using Backbone.Tooling;
using Backbone.UnitTestTools.Data;
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
    public async Task Message_body_can_be_retrived_from_blob_storage_if_unavailable_in_database()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));

        var rndIdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var rndDeviceId = TestDataGenerator.CreateRandomDeviceId();

        var messageWithFilledBody = new Message(
            rndIdentityAddress,
            rndDeviceId,
            SystemTime.UtcNow,
            new byte[] { 1, 2, 3 },
            new Attachment[] {},
            new RecipientInformation[] {});

        var messageWithEmptyBody = new Message(
            rndIdentityAddress,
            rndDeviceId,
            SystemTime.UtcNow,
            new byte[] { },
            new Attachment[] {},
            new RecipientInformation[] {});

        var messages = new List<Message>() {
            messageWithFilledBody,
            messageWithEmptyBody
        };

        await _arrangeContext.Messages.AddRangeAsync(messages);
        await _arrangeContext.SaveChangesAsync();

        var fakeBlobStorage = A.Fake<IBlobStorage>();

        A.CallTo(() => fakeBlobStorage.FindAsync(
                A<string>._, A<string>._))
            .Returns(new byte[] { 1, 2, 3 });
        
        var messageRepository = new MessagesRepository(_actContext, fakeBlobStorage, A.Fake<IOptions<BlobOptions>>());

        // Act
        var acting = messageRepository.FindMessagesWithIds(
                new List<MessageId>(), 
                rndIdentityAddress, 
                A.Fake<PaginationFilter>(), 
                CancellationToken.None)
            .Result.ItemsOnPage.ToList();

        // Assert
        acting.Count.Should().Be(messages.Count);

        foreach (var message in acting)
        {
            message.Body.Should().NotBeEmpty();
        }
    }
}
