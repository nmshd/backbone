using Backbone.Modules.Quotas.Application.Identities.Queries.GetIdentityQuotasByAddress;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.Tests.TestDoubles;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
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
        var result = await handler.Handle(new GetIdentityQuotasByAddressQuery(identity.Address), CancellationToken.None);

        // Assert
        result.IndividualQuotas.Should().HaveCount(1);
        result.TierQuotas.Should().HaveCount(1);

        result.IndividualQuotas.First().Max.Should().Be(max);
        result.IndividualQuotas.First().Period.Should().Be(period);
        result.IndividualQuotas.First().Metric.Key.Should().Be(metric.Key.Value);
        result.IndividualQuotas.First().Metric.DisplayName.Should().Be(metric.DisplayName);

        result.TierQuotas.First().Max.Should().Be(max);
        result.TierQuotas.First().Period.Should().Be(period);
        result.TierQuotas.First().Metric.Key.Should().Be(metric.Key.Value);
        result.TierQuotas.First().Metric.DisplayName.Should().Be(metric.DisplayName);
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
        var acting = async () => await handler.Handle(new GetIdentityQuotasByAddressQuery("some-inexistent-identity-address"), CancellationToken.None);

        // Assert
        await acting.Should().ThrowAsync<NotFoundException>();
    }

    private Handler CreateHandler(IIdentitiesRepository identitiesRepository, IMetricsRepository metricsRepository)
    {
        return new Handler(identitiesRepository, metricsRepository);
    }
}
