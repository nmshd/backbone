using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Quotas.Application.Identities.Commands.CreateQuotaForIdentity;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Application.Tests.TestDoubles;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Extensions;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;
using MetricKey = Backbone.Modules.Quotas.Domain.Aggregates.Metrics.MetricKey;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Quotas.CreateQuotaForIdentity;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Creates_quota_for_identity()
    {
        // Arrange
        const int max = 5;
        const QuotaPeriod period = QuotaPeriod.Month;
        var metricKey = MetricKey.NumberOfSentMessages.Value;

        var tierId = TierId.Parse("TIRsomeTierId1111111");
        var identity = new Identity(CreateRandomIdentityAddress(), tierId);
        var command = new CreateQuotaForIdentityCommand(identity.Address, metricKey, max, period);

        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.Find(identity.Address, A<CancellationToken>._, A<bool>._)).Returns(identity);

        var metricsRepository = new FindMetricsStubRepository(new Metric(MetricKey.NumberOfSentMessages, "Number Of Sent Messages"));
        var handler = CreateHandler(identitiesRepository, metricsRepository);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.Id.Should().NotBeNullOrEmpty();
        response.Period.Should().Be(period);
        response.Max.Should().Be(max);
        response.Metric.Key.Should().Be(metricKey);

        A.CallTo(() => identitiesRepository.Update(A<Identity>.That.Matches(t =>
                t.Address == identity.Address &&
                t.TierId == tierId &&
                t.IndividualQuotas.Count == 1)
            , CancellationToken.None)
        ).MustHaveHappened();
    }

    [Fact]
    public void Create_quota_with_invalid_metric_key_throws_domain_exception()
    {
        // Arrange
        var identity = new Identity(CreateRandomIdentityAddress(), TierId.Parse("TIRsomeTierId1111111"));
        var command = new CreateQuotaForIdentityCommand(identity.Address, "An-Invalid-Metric-Key", 5, QuotaPeriod.Month);
        var metricsRepository = new FindMetricsStubRepository(new Metric(MetricKey.NumberOfSentMessages, "Number Of Sent Messages"));

        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.Find(identity.Address, A<CancellationToken>._, A<bool>._)).Returns(identity);

        var handler = CreateHandler(identitiesRepository, metricsRepository);

        // Act
        Func<Task> acting = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        acting.Should().AwaitThrowAsync<DomainException>().Which.Code.Should().Be("error.platform.quotas.unsupportedMetricKey");
    }

    [Fact]
    public void Create_quota_for_non_existent_identity_throws_not_found_exception()
    {
        // Arrange
        var command = new CreateQuotaForIdentityCommand(IdentityAddress.Parse(CreateRandomIdentityAddress()), "An-Invalid-Metric-Key", 5, QuotaPeriod.Month);
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.Find(A<string>._, A<CancellationToken>._, A<bool>._)).Returns((Identity?)null);
        var metricsRepository = new FindMetricsStubRepository(new Metric(MetricKey.NumberOfSentMessages, "Number Of Sent Messages"));
        var handler = CreateHandler(identitiesRepository, metricsRepository);

        // Act
        Func<Task> acting = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = acting.Should().AwaitThrowAsync<NotFoundException>().Which;
        exception.Message.Should().StartWith("Identity");
        exception.Code.Should().Be("error.platform.recordNotFound");
    }

    [Fact]
    public async Task Updates_metric_statuses_after_creating_quota_for_identity()
    {
        // Arrange
        var metricKey = MetricKey.NumberOfSentMessages;
        var tierId = TierId.Parse("TIRsomeTierId1111111");
        var identity = new Identity(CreateRandomIdentityAddress(), tierId);
        var command = new CreateQuotaForIdentityCommand(identity.Address, metricKey.Value, 5, QuotaPeriod.Month);

        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.Find(identity.Address, A<CancellationToken>._, A<bool>._)).Returns(identity);
        var metricsRepository = new FindMetricsStubRepository(new Metric(MetricKey.NumberOfSentMessages, "Number Of Sent Messages"));
        var metricStatusesService = A.Fake<IMetricStatusesService>();
        var handler = CreateHandler(identitiesRepository, metricsRepository, metricStatusesService);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => metricStatusesService.RecalculateMetricStatuses(
            A<List<string>>.That.Matches(x => x.Contains(identity.Address)),
            A<List<MetricKey>>.That.Contains(metricKey),
            A<CancellationToken>._)
        ).MustHaveHappened();
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, IMetricsRepository metricsRepository, IMetricStatusesService? metricStatusesService = null)
    {
        var logger = A.Fake<ILogger<Handler>>();
        return new Handler(identitiesRepository, logger, metricsRepository, metricStatusesService ?? A.Fake<IMetricStatusesService>());
    }
}
