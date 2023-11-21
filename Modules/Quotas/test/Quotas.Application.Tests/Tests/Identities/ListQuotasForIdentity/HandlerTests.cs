﻿using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
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

        var max = 5;
        var period = QuotaPeriod.Month;

        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(metric1.Key, max, period));
        identity.CreateIndividualQuota(metric2.Key, max, period);

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(identity.Address);

        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => fakeIdentitiesRepository.Find(identity.Address, A<CancellationToken>._, A<bool>._)).Returns(identity);

        var fakeMetricsRepository = A.Fake<IMetricsRepository>();
        A.CallTo(() => fakeMetricsRepository.FindAll(CancellationToken.None, false)).Returns(new List<Metric> { metric1, metric2 });

        var fakeMetricCalculationFactory = new FakeMetricCalculatorFactory(numberOfSentMessages: 1, numberOfTokens: 1);

        var handler = new Handler(fakeUserContext, fakeIdentitiesRepository, fakeMetricsRepository, fakeMetricCalculationFactory);

        // Act
        var quotas = (await handler.Handle(new ListQuotasForIdentityQuery(), CancellationToken.None)).ToList();

        // Assert
        quotas.Should().HaveCount(2);

        var tierQuota = quotas.Single(q => q.Source == QuotaSource.Tier);
        var individualQuota = quotas.Single(q => q.Source == QuotaSource.Individual);

        tierQuota.Should().NotBeNull();
        tierQuota.Max.Should().Be(5);
        tierQuota.Usage.Should().Be(1);
        tierQuota.Period.Should().Be(identity.TierQuotas.First().Period.ToString());
        tierQuota.Metric.Key.Should().Be(metric1.Key.Value);
        tierQuota.Metric.DisplayName.Should().Be(metric1.DisplayName);

        individualQuota.Should().NotBeNull();
        individualQuota.Max.Should().Be(5);
        individualQuota.Usage.Should().Be(1);
        individualQuota.Period.Should().Be(identity.IndividualQuotas.First().Period.ToString());
        individualQuota.Metric.Key.Should().Be(metric2.Key.Value);
        individualQuota.Metric.DisplayName.Should().Be(metric2.DisplayName);
    }
}
