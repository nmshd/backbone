using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Quotas.Application.Identities.Queries.GetIdentity;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.Tests.TestDoubles;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.Metrics;
using Backbone.UnitTestTools.Extensions;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities.Queries.GetIdentity;

public class HandlerTests
{
    [Fact]
    public async void Gets_identity_quotas_by_address()
    {
        // Arrange
        var metric = new Metric(MetricKey.NumberOfSentMessages, "Number Of Sent Messages");
        var identity = new Identity("some-identity-address", new TierId("SomeTierId"));

        const int max = 5;
        const QuotaPeriod period = QuotaPeriod.Month;

        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(metric.Key, max, period));
        identity.CreateIndividualQuota(metric.Key, max, period);

        var stubMetricsRepository = new FindAllWithKeysMetricsStubRepository(new List<Metric> { metric });

        var stubIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => stubIdentitiesRepository.Find(A<string>._, A<CancellationToken>._, A<bool>._)).Returns(identity);

        var handler = CreateHandler(stubIdentitiesRepository, stubMetricsRepository);

        // Act
        var result = await handler.Handle(new GetIdentityQuery(identity.Address), CancellationToken.None);

        // Assert
        result.Address.Should().Be(identity.Address);
        result.Quotas.Should().HaveCount(2);

        result.Quotas.First().Max.Should().Be(max);
        result.Quotas.First().Period.Should().Be(period.ToString());
        result.Quotas.First().Source.Should().Be(QuotaSource.Individual);
        result.Quotas.First().Metric.Key.Should().Be(metric.Key.Value);
        result.Quotas.First().Metric.DisplayName.Should().Be(metric.DisplayName);

        result.Quotas.Second().Max.Should().Be(max);
        result.Quotas.Second().Period.Should().Be(period.ToString());
        result.Quotas.Second().Source.Should().Be(QuotaSource.Tier);
        result.Quotas.Second().Metric.Key.Should().Be(metric.Key.Value);
        result.Quotas.Second().Metric.DisplayName.Should().Be(metric.DisplayName);
    }

    [Fact]
    public void Fails_when_no_identity_found()
    {
        // Arrange
        var stubIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => stubIdentitiesRepository.Find(A<string>._, A<CancellationToken>._, A<bool>._)).Returns((Identity?)null);

        var handler = CreateHandler(stubIdentitiesRepository);

        // Act
        Func<Task> acting = async () => await handler.Handle(new GetIdentityQuery("some-inexistent-identity-address"), CancellationToken.None);

        // Assert
        var exception = acting.Should().AwaitThrowAsync<NotFoundException>().Which;
        exception.Message.Should().StartWith("Identity");
        exception.Code.Should().Be("error.platform.recordNotFound");
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository)
    {
        var dummyMetricsRepository = A.Dummy<IMetricsRepository>();
        return CreateHandler(identitiesRepository, dummyMetricsRepository);
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, IMetricsRepository metricsRepository)
    {
        var dummyMetricCalculatorFactory = A.Dummy<MetricCalculatorFactory>();
        return new Handler(identitiesRepository, metricsRepository, dummyMetricCalculatorFactory);
    }
}
