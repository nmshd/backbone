using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;
using Enmeshed.UnitTestTools.Data;
using Enmeshed.UnitTestTools.TestDoubles.Fakes;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;
using MessageEntity = Backbone.Modules.Messages.Domain.Entities.Message;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Repositories;
public class MessagesRepositoryTests
{
    private readonly IdentityAddress _identityAddress1 = TestDataGenerator.CreateRandomIdentityAddress();
    private readonly IdentityAddress _identityAddress2 = TestDataGenerator.CreateRandomIdentityAddress();

    private readonly MessagesDbContext _messagesArrangeContext;
    private readonly QuotasDbContext _actContext;

    private static readonly DateTime YESTERDAY = DateTime.UtcNow.AddDays(-1);
    private static readonly DateTime TOMORROW = DateTime.UtcNow.AddDays(1);
    private static readonly DateTime LAST_YEAR = DateTime.UtcNow.AddYears(-1);
    private static readonly DateTime NEXT_YEAR = DateTime.UtcNow.AddYears(1);

    public MessagesRepositoryTests()
    {
        AssertionScope.Current.FormattingOptions.MaxLines = 1000;

        (_messagesArrangeContext, _, _, var connection) = FakeDbContextFactory.CreateDbContexts<MessagesDbContext>();
        (_, _, _actContext, _) = FakeDbContextFactory.CreateDbContexts<QuotasDbContext>(connection);
    }

    [Fact]
    public async void Counts_entities_within_timeframe_hour_quotaPeriod()
    {
        // Arrange
        var messages = new List<MessageEntity>() {
            CreateMessage(DateTime.Now, _identityAddress1),
            CreateMessage(YESTERDAY, _identityAddress1),
            CreateMessage(TOMORROW, _identityAddress1)
        };
        await _messagesArrangeContext.Messages.AddRangeAsync(messages);
        await _messagesArrangeContext.SaveChangesAsync();

        var repository = new MessagesRepository(_actContext);
        var quotaPeriod = QuotaPeriod.Hour;

        // Act
        var count = await repository.Count(_identityAddress1, quotaPeriod.CalculateBegin(), quotaPeriod.CalculateEnd(), CancellationToken.None);

        // Assert
        count.Should().Be(1);
    }

    [Fact]
    public async void Counts_entities_within_timeframe_month_quotaPeriod()
    {
        // Arrange
        var halfOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 15);
        SystemTime.Set(halfOfMonth);

        var messages = new List<MessageEntity>() {
            CreateMessage(DateTime.Now, _identityAddress1),
            CreateMessage(YESTERDAY, _identityAddress1),
            CreateMessage(TOMORROW, _identityAddress1),
            CreateMessage(LAST_YEAR, _identityAddress1),
            CreateMessage(NEXT_YEAR, _identityAddress1)
        };
        await _messagesArrangeContext.Messages.AddRangeAsync(messages);
        await _messagesArrangeContext.SaveChangesAsync();

        var repository = new MessagesRepository(_actContext);
        var quotaPeriod = QuotaPeriod.Month;

        // Act
        var count = await repository.Count(_identityAddress1, quotaPeriod.CalculateBegin(), quotaPeriod.CalculateEnd(), CancellationToken.None);

        // Assert
        count.Should().Be(3);
    }

    [Fact]
    public async void Counts_entities_total_quotaPeriod()
    {
        // Arrange
        var messages = new List<MessageEntity>() {
            CreateMessage(DateTime.Now, _identityAddress1),
            CreateMessage(TOMORROW, _identityAddress1),
            CreateMessage(NEXT_YEAR, _identityAddress1)
        };
        await _messagesArrangeContext.Messages.AddRangeAsync(messages);
        await _messagesArrangeContext.SaveChangesAsync();

        var repository = new MessagesRepository(_actContext);
        var quotaPeriod = QuotaPeriod.Total;

        // Act
        var count = await repository.Count(_identityAddress1, quotaPeriod.CalculateBegin(), quotaPeriod.CalculateEnd(), CancellationToken.None);

        // Assert
        count.Should().Be(3);
    }


    [Fact]
    public async void Counts_entities_only_for_requested_identityAddress()
    {
        // Arrange
        var messages = new List<MessageEntity>() {
            CreateMessage(DateTime.Now, _identityAddress1),
            CreateMessage(TOMORROW, _identityAddress2),
            CreateMessage(NEXT_YEAR, _identityAddress1)
        };
        await _messagesArrangeContext.Messages.AddRangeAsync(messages);
        await _messagesArrangeContext.SaveChangesAsync();

        var repository = new MessagesRepository(_actContext);
        var quotaPeriod = QuotaPeriod.Total;

        // Act
        var count = await repository.Count(_identityAddress1, quotaPeriod.CalculateBegin(), quotaPeriod.CalculateEnd(), CancellationToken.None);

        // Assert
        count.Should().Be(2);
    }

    private MessageEntity CreateMessage(DateTime createdAt, IdentityAddress identityAddress)
    {
        return new(
            identityAddress,
            DeviceId.New(),
            null,
            Array.Empty<byte>(),
            new List<Attachment>(),
            new List<RecipientInformation>()
            )
        {
            CreatedAt = createdAt
        };
    }
}
