using Backbone.Modules.Quotas.Application.Identities.Queries.GetIdentity;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.Tests.TestDoubles;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.UnitTestTools.Extensions;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities.GetIdentityQuotasByAddress;
public class HandlerTests
{
    [Fact]
    public async void Gets_identity_quotas_by_address()
    {
        // Arrange
        var metric = new Metric(MetricKey.NumberOfSentMessages, "Number Of Sent Messages");
        var identity = new Identity("some-identity-address", new TierId("SomeTierId"));

        var max = 5;
        var period = QuotaPeriod.Month;

        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(metric.Key, max, period));
        identity.CreateIndividualQuota(metric.Key, max, period);

        var stubMetricsRepository = new FindAllWithKeysMetricsStubRepository(new List<Metric> { metric });

        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.Find(A<string>._, A<CancellationToken>._, A<bool>._)).Returns(identity);

        var handler = CreateHandler(identitiesRepository, stubMetricsRepository);

        // Act
        var result = await handler.Handle(new GetIdentityQuery(identity.Address), CancellationToken.None);

        // Assert
        result.Address.Should().Be(identity.Address);
        result.Quotas.Should().HaveCount(2);

        result.Quotas.First().Max.Should().Be(max);
        result.Quotas.First().Period.Should().Be(period.ToString());
        result.Quotas.First().Metric.Key.Should().Be(metric.Key.Value);
        result.Quotas.First().Metric.DisplayName.Should().Be(metric.DisplayName);

        result.Quotas.Second().Max.Should().Be(max);
        result.Quotas.Second().Period.Should().Be(period.ToString());
        result.Quotas.Second().Metric.Key.Should().Be(metric.Key.Value);
        result.Quotas.Second().Metric.DisplayName.Should().Be(metric.DisplayName);
    }

    [Fact]
    public async void Fails_when_no_identity_found()
    {
        // Arrange
        var metricsRepository = A.Fake<IMetricsRepository>();
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.Find(A<string>._, A<CancellationToken>._, A<bool>._)).Returns((Identity)null);

        var handler = CreateHandler(identitiesRepository, metricsRepository);

        // Act
        Func<Task> acting = async () => await handler.Handle(new GetIdentityQuery("some-inexistent-identity-address"), CancellationToken.None);

        // Assert
        var exception = acting.Should().AwaitThrowAsync<NotFoundException>().Which;
        exception.Message.Should().StartWith("Identity");
        exception.Code.Should().Be("error.platform.recordNotFound");
    }

    private Handler CreateHandler(IIdentitiesRepository identitiesRepository, IMetricsRepository metricsRepository)
    {
        return new Handler(identitiesRepository, metricsRepository);
    }
}
