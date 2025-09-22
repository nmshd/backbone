using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;
using Backbone.Tooling;
using Backbone.UnitTestTools.TestDoubles.Fakes;

namespace Backbone.Modules.Quotas.Infrastructure.Tests.Tests.Repositories;

public class MessagesRepositoryTests : AbstractTestsBase
{
    private readonly IdentityAddress _identityAddress1 = CreateRandomIdentityAddress();
    private readonly IdentityAddress _identityAddress2 = CreateRandomIdentityAddress();

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
        var connection = FakeDbContextFactory.CreateDbConnection();
        (_messagesArrangeContext, _, _) = FakeDbContextFactory.CreateDbContexts<MessagesDbContext>(connection);
        (_, _actContext, _) = FakeDbContextFactory.CreateDbContexts<QuotasDbContext>(connection);

        SystemTime.Set(TODAY);
    }

    [Fact]
    public async Task Counts_entities_within_timeframe_hour_quotaPeriod()
    {
        // Arrange
        var messages = new List<Message>
        {
            CreateMessage(TODAY, _identityAddress1),
            CreateMessage(YESTERDAY, _identityAddress1),
            CreateMessage(TOMORROW, _identityAddress1)
        };
        await _messagesArrangeContext.Messages.AddRangeAsync(messages, TestContext.Current.CancellationToken);
        await _messagesArrangeContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new MessagesRepository(_actContext);
        const QuotaPeriod quotaPeriod = QuotaPeriod.Hour;

        // Act
        var count = await repository.Count(_identityAddress1, quotaPeriod.CalculateBegin(SystemTime.UtcNow), quotaPeriod.CalculateEnd(SystemTime.UtcNow), CancellationToken.None);

        // Assert
        count.ShouldBe(1u);
    }

    [Fact]
    public async Task Counts_entities_within_timeframe_month_quotaPeriod()
    {
        // Arrange
        var messages = new List<Message>
        {
            CreateMessage(TODAY, _identityAddress1),
            CreateMessage(YESTERDAY, _identityAddress1),
            CreateMessage(TOMORROW, _identityAddress1),
            CreateMessage(LAST_YEAR, _identityAddress1),
            CreateMessage(NEXT_YEAR, _identityAddress1)
        };
        await _messagesArrangeContext.Messages.AddRangeAsync(messages, TestContext.Current.CancellationToken);
        await _messagesArrangeContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new MessagesRepository(_actContext);
        const QuotaPeriod quotaPeriod = QuotaPeriod.Month;

        // Act
        var count = await repository.Count(_identityAddress1, quotaPeriod.CalculateBegin(SystemTime.UtcNow), quotaPeriod.CalculateEnd(SystemTime.UtcNow), CancellationToken.None);

        // Assert
        count.ShouldBe(3u);
    }

    [Fact]
    public async Task Counts_entities_total_quotaPeriod()
    {
        // Arrange
        var messages = new List<Message>
        {
            CreateMessage(TODAY, _identityAddress1),
            CreateMessage(TOMORROW, _identityAddress1),
            CreateMessage(NEXT_YEAR, _identityAddress1)
        };
        await _messagesArrangeContext.Messages.AddRangeAsync(messages, TestContext.Current.CancellationToken);
        await _messagesArrangeContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new MessagesRepository(_actContext);
        const QuotaPeriod quotaPeriod = QuotaPeriod.Total;

        // Act
        var count = await repository.Count(_identityAddress1, quotaPeriod.CalculateBegin(SystemTime.UtcNow), quotaPeriod.CalculateEnd(SystemTime.UtcNow), CancellationToken.None);

        // Assert
        count.ShouldBe(3u);
    }

    [Fact]
    public async Task Counts_entities_only_for_requested_identityAddress()
    {
        // Arrange
        var messages = new List<Message>
        {
            CreateMessage(TODAY, _identityAddress1),
            CreateMessage(TOMORROW, _identityAddress2),
            CreateMessage(NEXT_YEAR, _identityAddress1)
        };
        await _messagesArrangeContext.Messages.AddRangeAsync(messages, TestContext.Current.CancellationToken);
        await _messagesArrangeContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new MessagesRepository(_actContext);
        const QuotaPeriod quotaPeriod = QuotaPeriod.Total;

        // Act
        var count = await repository.Count(_identityAddress1, quotaPeriod.CalculateBegin(SystemTime.UtcNow), quotaPeriod.CalculateEnd(SystemTime.UtcNow), CancellationToken.None);

        // Assert
        count.ShouldBe(2u);
    }

    private static Message CreateMessage(DateTime createdAt, IdentityAddress identityAddress)
    {
        var savedDateTime = SystemTime.UtcNow;

        SystemTime.Set(createdAt);

        var message = new Message(
            identityAddress,
            DeviceId.New(),
            [],
            [],
            []
        );

        SystemTime.Set(savedDateTime);

        return message;
    }
}
