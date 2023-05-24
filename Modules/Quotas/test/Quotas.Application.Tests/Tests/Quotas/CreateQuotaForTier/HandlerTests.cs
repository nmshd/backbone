using Backbone.Modules.Quotas.Application.AutoMapper;
using Backbone.Modules.Quotas.Application.Quotas.Commands.CreateQuotaForTier;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Quotas.CreateQuotaForTier;

public class HandlerTests
{
    public HandlerTests()
    {
        AssertionScope.Current.FormattingOptions.MaxLines = 1000;
    }

    [Fact]
    public async Task Successfully_creates_quota_for_tier()
    {
        // Arrange
        var tierId = "TIRsomeTierId1111111";
        var max = 5;
        var period = QuotaPeriod.Month;
        var command = new CreateQuotaForTierCommand(tierId, MetricKey.NumberOfSentMessages, max, period);
        var tier = new Tier(tierId, "some-tier-name");
        var tiers = new List<Tier> { tier };
        var mockTiersRepository = new MockTiersRepository(tiers);
        var metricsRepository = new FindMetricsStubRepository(new Metric(MetricKey.NumberOfSentMessages, "Number Of Sent Messages"));
        var handler = CreateHandler(mockTiersRepository, metricsRepository);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.Id.Should().NotBeNullOrEmpty();
        response.Period.Should().Be(period);
        response.Max.Should().Be(max);
        response.Metric.Key.Should().Be(MetricKey.NumberOfSentMessages);

        mockTiersRepository.WasUpdateCalled.Should().BeTrue();
        mockTiersRepository.WasUpdateCalledWith.Quotas.Count.Should().Be(1);

    }

    [Fact]
    public async Task Throws_not_found_exception_when_no_tier_with_given_id_is_found()
    {
        // Arrange
        var tierId = "TIRoneInexistentTier";
        var max = 5;
        var period = QuotaPeriod.Month;
        var command = new CreateQuotaForTierCommand(tierId, MetricKey.NumberOfSentMessages, max, period);
        var tiers = new List<Tier> { new Tier("TIRsomeTierId1111111", "some-tier-name") };
        var mockTiersRepository = new MockTiersRepository(tiers);
        var metricsRepository = new FindMetricsStubRepository(new Metric(MetricKey.NumberOfSentMessages, "Number Of Sent Messages"));
        var handler = CreateHandler(mockTiersRepository, metricsRepository);

        // Act
        Func<Task> acting = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await acting.Should().ThrowAsync<NotFoundException>();
    }

    private Handler CreateHandler(MockTiersRepository tiersRepository, FindMetricsStubRepository metricsRepository)
    {
        var logger = A.Fake<ILogger<Handler>>();
        var eventBus = A.Fake<IEventBus>();
        var mapper = AutoMapperProfile.CreateMapper();

        return new Handler(tiersRepository, logger, eventBus, metricsRepository, mapper);
    }
}
