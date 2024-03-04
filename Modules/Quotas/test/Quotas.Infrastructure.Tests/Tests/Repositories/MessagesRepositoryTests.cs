using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;
using Backbone.Tooling;
using Backbone.UnitTestTools.Data;
using Backbone.UnitTestTools.TestDoubles.Fakes;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Backbone.Modules.Quotas.Infrastructure.Tests.Tests.Repositories;

public class MessagesRepositoryTests
{
    private readonly IdentityAddress _identityAddress1 = TestDataGenerator.CreateRandomIdentityAddress();
    private readonly IdentityAddress _identityAddress2 = TestDataGenerator.CreateRandomIdentityAddress();

    private readonly MessagesDbContext _messagesArrangeContext;
    private readonly QuotasDbContext _actContext;

    /**
     * <summary>
     * TODAY is set to the 15th of the current month so that we can use notions
     * such as TOMORROW or YESTERDAY and be certain that those dates fall on the same Month.
     * </summary>
     */
    private static readonly DateTime TODAY = new(2020, 02, 15, 10, 30, 00);

    private static readonly DateTime YESTERDAY = TODAY.AddDays(-1);
    private static readonly DateTime TOMORROW = TODAY.AddDays(1);
    private static readonly DateTime LAST_YEAR = TODAY.AddYears(-1);
    private static readonly DateTime NEXT_YEAR = TODAY.AddYears(1);

    public MessagesRepositoryTests()
    {
        AssertionScope.Current.FormattingOptions.MaxLines = 1000;

        var connection = FakeDbContextFactory.CreateDbConnection();
        (_messagesArrangeContext, _, _) = FakeDbContextFactory.CreateDbContexts<MessagesDbContext>(connection);
        (_, _, _actContext) = FakeDbContextFactory.CreateDbContexts<QuotasDbContext>(connection);

        SystemTime.Set(TODAY);
    }

    [Fact]
    public async Task Counts_entities_within_timeframe_hour_quotaPeriod()
    {
        // Arrange
        var messages = new List<Message>()
        {
            CreateMessage(TODAY, _identityAddress1),
            CreateMessage(YESTERDAY, _identityAddress1),
            CreateMessage(TOMORROW, _identityAddress1)
        };
        await _messagesArrangeContext.Messages.AddRangeAsync(messages);
        await _messagesArrangeContext.SaveChangesAsync();

        var repository = new MessagesRepository(_actContext);
        const QuotaPeriod quotaPeriod = QuotaPeriod.Hour;

        // Act
        var count = await repository.Count(_identityAddress1, quotaPeriod.CalculateBegin(), quotaPeriod.CalculateEnd(), CancellationToken.None);

        // Assert
        count.Should().Be(1);
    }

    [Fact]
    public async Task Counts_entities_within_timeframe_month_quotaPeriod()
    {
        // Arrange
        var messages = new List<Message>()
        {
            CreateMessage(TODAY, _identityAddress1),
            CreateMessage(YESTERDAY, _identityAddress1),
            CreateMessage(TOMORROW, _identityAddress1),
            CreateMessage(LAST_YEAR, _identityAddress1),
            CreateMessage(NEXT_YEAR, _identityAddress1)
        };
        await _messagesArrangeContext.Messages.AddRangeAsync(messages);
        await _messagesArrangeContext.SaveChangesAsync();

        var repository = new MessagesRepository(_actContext);
        const QuotaPeriod quotaPeriod = QuotaPeriod.Month;

        // Act
        var count = await repository.Count(_identityAddress1, quotaPeriod.CalculateBegin(), quotaPeriod.CalculateEnd(), CancellationToken.None);

        // Assert
        count.Should().Be(3);
    }

    [Fact]
    public async Task Counts_entities_total_quotaPeriod()
    {
        // Arrange
        var messages = new List<Message>()
        {
            CreateMessage(TODAY, _identityAddress1),
            CreateMessage(TOMORROW, _identityAddress1),
            CreateMessage(NEXT_YEAR, _identityAddress1)
        };
        await _messagesArrangeContext.Messages.AddRangeAsync(messages);
        await _messagesArrangeContext.SaveChangesAsync();

        var repository = new MessagesRepository(_actContext);
        const QuotaPeriod quotaPeriod = QuotaPeriod.Total;

        // Act
        var count = await repository.Count(_identityAddress1, quotaPeriod.CalculateBegin(), quotaPeriod.CalculateEnd(), CancellationToken.None);

        // Assert
        count.Should().Be(3);
    }

    [Fact]
    public async Task Counts_entities_only_for_requested_identityAddress()
    {
        // Arrange
        var messages = new List<Message>()
        {
            CreateMessage(TODAY, _identityAddress1),
            CreateMessage(TOMORROW, _identityAddress2),
            CreateMessage(NEXT_YEAR, _identityAddress1)
        };
        await _messagesArrangeContext.Messages.AddRangeAsync(messages);
        await _messagesArrangeContext.SaveChangesAsync();

        var repository = new MessagesRepository(_actContext);
        const QuotaPeriod quotaPeriod = QuotaPeriod.Total;

        // Act
        var count = await repository.Count(_identityAddress1, quotaPeriod.CalculateBegin(), quotaPeriod.CalculateEnd(), CancellationToken.None);

        // Assert
        count.Should().Be(2);
    }

    private static Message CreateMessage(DateTime createdAt, IdentityAddress identityAddress)
    {
        var savedDateTime = SystemTime.UtcNow;

        SystemTime.Set(createdAt);

        var message = new Message(
            identityAddress,
            DeviceId.New(),
            null,
            Array.Empty<byte>(),
            [],
            []
        );

        SystemTime.Set(savedDateTime);

        return message;
    }
}
