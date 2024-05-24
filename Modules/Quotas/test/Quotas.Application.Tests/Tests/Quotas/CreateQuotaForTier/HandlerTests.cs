using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.Tests.TestDoubles;
using Backbone.Modules.Quotas.Application.Tiers.Commands.CreateQuotaForTier;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Xunit;
using MetricKey = Backbone.Modules.Quotas.Domain.Aggregates.Metrics.MetricKey;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Quotas.CreateQuotaForTier;

public class HandlerTests
{
    public HandlerTests()
    {
        AssertionScope.Current.FormattingOptions.MaxLines = 1000;
    }

    [Fact]
    public async Task Creates_quota_for_tier()
    {
        // Arrange
        var tierId = TierId.Parse("TIRsomeTierId1111111");
        const int max = 5;
        const QuotaPeriod period = QuotaPeriod.Month;
        var metricKey = MetricKey.NumberOfSentMessages.Value;
        var command = new CreateQuotaForTierCommand(tierId, metricKey, max, period);
        var tier = new Tier(tierId, "some-tier-name");

        var tierRepository = A.Fake<ITiersRepository>();
        A.CallTo(() => tierRepository.Find(tierId, A<CancellationToken>._, A<bool>._)).Returns(tier);

        var metricsRepository = new FindMetricsStubRepository(new Metric(MetricKey.NumberOfSentMessages, "Number Of Sent Messages"));
        var handler = CreateHandler(tierRepository, metricsRepository);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.Id.Should().NotBeNullOrEmpty();
        response.Period.Should().Be(period);
        response.Max.Should().Be(max);
        response.Metric.Key.Should().Be(metricKey);

        A.CallTo(() => tierRepository.Update(A<Tier>.That.Matches(t =>
                t.Id == tierId &&
                t.Quotas.Count == 1)
            , CancellationToken.None)
        ).MustHaveHappened();
    }

    private Handler CreateHandler(ITiersRepository tiersRepository, FindMetricsStubRepository metricsRepository)
    {
        var logger = A.Fake<ILogger<Handler>>();

        return new Handler(tiersRepository, logger, metricsRepository);
    }
}
