using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Messages;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;
using Enmeshed.UnitTestTools.TestDoubles.Fakes;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Repositories;
public class MessagesRepositoryTests
{
    private const string IDENTITY_ADDRESS_1 = "IdentityAddress-1";
    private readonly QuotasDbContext _arrangeContext;
    private readonly QuotasDbContext _actContext;

    private static readonly DateTime YESTERDAY = DateTime.UtcNow.AddDays(-1);
    private static readonly DateTime TOMORROW = DateTime.UtcNow.AddDays(1);

    public MessagesRepositoryTests()
    {
        AssertionScope.Current.FormattingOptions.MaxLines = 1000;

        (_arrangeContext, _, _actContext) = FakeDbContextFactory.CreateDbContexts<QuotasDbContext>();
    }

    [Fact]
    public async void Counts_entities_within_timeframe()
    {
        // Arrange
        var messages = new List<Message>() {
            new("MessageId", IDENTITY_ADDRESS_1, DateTime.Now),
            new("MessageId", IDENTITY_ADDRESS_1, YESTERDAY)
        };
        await _arrangeContext.Messages.AddAsync(messages.First());
        await _arrangeContext.SaveChangesAsync();
        var repo = new MessagesRepository(_actContext);
        var quotaPeriod = QuotaPeriod.Hour;

        // Act
        var count = await repo.Count(IDENTITY_ADDRESS_1, quotaPeriod.CalculateBegin(), quotaPeriod.CalculateEnd(), CancellationToken.None);

        // Assert
        count.Should().Be(1);
    }
}
