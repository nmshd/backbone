using Backbone.Modules.Quotas.Application.Identities.Commands.CreateQuotaForIdentity;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Application.Tests.TestDoubles;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using MetricKey = Backbone.Modules.Quotas.Domain.Aggregates.Metrics.MetricKey;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Quotas.CreateQuotaForIdentity;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Updates_metric_statuses_after_creating_quota_for_identity()
    {
        // Arrange
        var metricKey = MetricKey.NUMBER_OF_SENT_MESSAGES;
        var tierId = TierId.Parse("TIRsomeTierId1111111");
        var identity = new Identity(CreateRandomIdentityAddress(), tierId);
        var command = new CreateQuotaForIdentityCommand(identity.Address, metricKey.Value, 5, QuotaPeriod.Month);

        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.Find(identity.Address, A<CancellationToken>._, A<bool>._)).Returns(identity);
        var metricsRepository = new FindMetricsStubRepository(new Metric(MetricKey.NUMBER_OF_SENT_MESSAGES, "Number Of Sent Messages"));
        var metricStatusesService = A.Fake<IMetricStatusesService>();
        var handler = CreateHandler(identitiesRepository, metricsRepository, metricStatusesService);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => metricStatusesService.RecalculateMetricStatuses(
            A<List<string>>.That.Matches(x => x.Contains(identity.Address)),
            A<List<MetricKey>>.That.Contains(metricKey)
            , MetricUpdateType.All,
            A<CancellationToken>._)
        ).MustHaveHappened();
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, IMetricsRepository metricsRepository, IMetricStatusesService? metricStatusesService = null)
    {
        var logger = A.Fake<ILogger<Handler>>();
        return new Handler(identitiesRepository, logger, metricsRepository, metricStatusesService ?? A.Fake<IMetricStatusesService>());
    }
}
