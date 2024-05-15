using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.Tests.Extensions;
using Backbone.Modules.Quotas.Application.Tiers.Commands.DeleteTierQuotaDefinition;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.DomainEvents.Outgoing;
using Backbone.UnitTestTools.FluentAssertions.Extensions;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Xunit;
using MetricKey = Backbone.Modules.Quotas.Domain.Aggregates.Metrics.MetricKey;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Quotas.DeleteTierQuotaDefinition;

public class HandlerTests
{
    public HandlerTests()
    {
        AssertionScope.Current.FormattingOptions.MaxLines = 1000;
    }

    [Fact]
    public async Task Triggers_TierQuotaDefinitionDeletedDomainEvent()
    {
        // Arrange
        var tierId = TierId.New();
        var tier = new Tier(tierId, "some-tier-name");

        tier.CreateQuota(MetricKey.NumberOfSentMessages, 5, QuotaPeriod.Month);
        var tierQuotaDefinitionId = tier.Quotas.First().Id;

        var command = new DeleteTierQuotaDefinitionCommand(tier.Id, tierQuotaDefinitionId);

        var tiersRepository = A.Fake<ITiersRepository>();
        A.CallTo(() => tiersRepository.Find(tierId, A<CancellationToken>._, A<bool>._)).Returns(tier);

        var handler = CreateHandler(tiersRepository);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var domainEvent = tier.Should().HaveASingleDomainEvent<TierQuotaDefinitionDeletedDomainEvent>(); //TODO: Timo (Should I make this a domain test as well or is this handler test enough?)
        domainEvent.TierId.Should().Be(tierId);
        domainEvent.TierQuotaDefinitionId.Should().Be(tierQuotaDefinitionId);
    }

    [Fact]
    public async Task Deletes_tier_quota_definition()
    {
        // Arrange
        var tierId = TierId.New();
        var tier = new Tier(tierId, "some-tier-name");

        tier.CreateQuota(MetricKey.NumberOfSentMessages, 5, QuotaPeriod.Month);

        var command = new DeleteTierQuotaDefinitionCommand(tier.Id, tier.Quotas.First().Id);

        var tiersRepository = A.Fake<ITiersRepository>();
        A.CallTo(() => tiersRepository.Find(tierId, A<CancellationToken>._, A<bool>._)).Returns(tier);

        var handler = CreateHandler(tiersRepository);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => tiersRepository.Update(A<Tier>.That.Matches(t =>
                t.Id == tierId &&
                t.Quotas.Count == 0)
            , CancellationToken.None)
        ).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Deletes_tier_quota_definition_with_multiple_quotas()
    {
        // Arrange
        var tierId = TierId.New();
        var tier = new Tier(tierId, "some-tier-name");

        tier.CreateQuota(MetricKey.NumberOfSentMessages, 5, QuotaPeriod.Month);
        tier.CreateQuota(MetricKey.NumberOfSentMessages, 10, QuotaPeriod.Year);
        tier.CreateQuota(MetricKey.NumberOfSentMessages, 15, QuotaPeriod.Total);

        var command = new DeleteTierQuotaDefinitionCommand(tier.Id, tier.Quotas.First().Id);

        var tiersRepository = A.Fake<ITiersRepository>();
        A.CallTo(() => tiersRepository.Find(tierId, A<CancellationToken>._, A<bool>._)).Returns(tier);

        var handler = CreateHandler(tiersRepository);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => tiersRepository.Update(A<Tier>.That.Matches(t =>
                t.Id == tierId &&
                t.Quotas.Count == 2)
            , CancellationToken.None)
        ).MustHaveHappened();
    }

    [Fact]
    public async Task Fails_to_delete_tier_quota_definition_for_missing_tier()
    {
        // Arrange
        var tierId = TierId.New();

        var command = new DeleteTierQuotaDefinitionCommand(tierId, "SomeTierQuotaDefinitionId");

        var tiersRepository = A.Fake<ITiersRepository>();
        A.CallTo(() => tiersRepository.Find(tierId, A<CancellationToken>._, A<bool>._)).Returns(Task.FromResult<Tier?>(null));

        var handler = CreateHandler(tiersRepository);

        // Act
        var acting = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await acting.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Fails_to_delete_tier_quota_definition_for_missing_quota()
    {
        // Arrange
        var tierId = TierId.New();
        var tier = new Tier(tierId, "some-tier-name");

        var command = new DeleteTierQuotaDefinitionCommand(tier.Id, "SomeTierQuotaDefinitionId");

        var tiersRepository = A.Fake<ITiersRepository>();
        A.CallTo(() => tiersRepository.Find(tierId, A<CancellationToken>._, A<bool>._)).Returns(tier);

        var handler = CreateHandler(tiersRepository);

        // Act
        var acting = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await acting.Should().ThrowAsync<DomainException>().WithErrorCode("error.platform.recordNotFound");
    }

    private Handler CreateHandler(ITiersRepository tiersRepository)
    {
        var logger = A.Fake<ILogger<Handler>>();

        return new Handler(tiersRepository, logger);
    }
}
