using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Quotas.Application.Identities.Queries.ListQuotasForIdentity;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FakeItEasy;
using FluentAssertions;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities.ListQuotasForIdentity;

public class HandlerTests
{
    [Fact]
    public async Task Returns_individual_and_tier_quotas_for_identity()
    {
        // Arrange
        var metric1 = new Metric(MetricKey.NumberOfSentMessages, "Number Of Sent Messages");
        var metric2 = new Metric(MetricKey.NumberOfTokens, "Number Of Tokens");
        var identity = new Identity(CreateRandomIdentityAddress(), new TierId("SomeTierId"));

        const int max = 5;
        const QuotaPeriod period = QuotaPeriod.Month;

        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(metric1.Key, max, period));
        identity.CreateIndividualQuota(metric2.Key, max, period);

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(identity.Address);

        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => fakeIdentitiesRepository.Find(identity.Address, A<CancellationToken>._, A<bool>._)).Returns(identity);

        var fakeMetricsRepository = A.Fake<IMetricsRepository>();
        A.CallTo(() => fakeMetricsRepository.FindAll(A<CancellationToken>._, false)).Returns(new List<Metric> { metric1, metric2 });

        var fakeMetricCalculationFactory = new FakeMetricCalculatorFactory(value: 1);

        var handler = new Handler(fakeUserContext, fakeIdentitiesRepository, fakeMetricsRepository, fakeMetricCalculationFactory);

        // Act
        var quotaGroupDTOs = (await handler.Handle(new ListQuotasForIdentityQuery(), CancellationToken.None)).ToList();
        var singleQuotaDTOs = quotaGroupDTOs.SelectMany(group => group.Quotas);

        // Assert
        singleQuotaDTOs.Should().HaveCount(2);

        singleQuotaDTOs.Should().Contain(q => q.MetricKey == metric1.Key.Value);
        singleQuotaDTOs.Should().Contain(q => q.MetricKey == metric2.Key.Value);

        var tierQuota = singleQuotaDTOs.Single(q => q.MetricKey == metric1.Key.Value);
        var individualQuota = singleQuotaDTOs.Single(q => q.MetricKey == metric2.Key.Value);

        tierQuota.Max.Should().Be(5);
        tierQuota.Usage.Should().Be(1);
        tierQuota.Period.Should().Be("Month");

        individualQuota.Max.Should().Be(5);
        individualQuota.Usage.Should().Be(1);
        individualQuota.Period.Should().Be("Month");
    }
}
