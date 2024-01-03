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

// ReSharper disable InconsistentNaming

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

        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(metric1.Key, 5, QuotaPeriod.Month));
        identity.CreateIndividualQuota(metric2.Key, 5, QuotaPeriod.Month);
        identity.CreateIndividualQuota(metric1.Key, 10, QuotaPeriod.Week);

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(identity.Address);

        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => fakeIdentitiesRepository.Find(identity.Address, A<CancellationToken>._, A<bool>._)).Returns(identity);

        var fakeMetricsRepository = A.Fake<IMetricsRepository>();
        A.CallTo(() => fakeMetricsRepository.FindAll(A<CancellationToken>._, false)).Returns(new List<Metric> { metric1, metric2 });

        var fakeMetricCalculatorFactory = new FakeMetricCalculatorFactory(numberOfSentMessages: 1, numberOfTokens: 2);

        var handler = new Handler(fakeUserContext, fakeIdentitiesRepository, fakeMetricsRepository, fakeMetricCalculatorFactory);

        // Act
        var quotaGroupDTOs = (await handler.Handle(new ListQuotasForIdentityQuery(), CancellationToken.None)).Items.ToList();

        // Assert
        quotaGroupDTOs.Should().HaveCount(2);

        quotaGroupDTOs.Should().ContainSingle(qg => qg.MetricKey == metric1.Key.Value);
        quotaGroupDTOs.Should().ContainSingle(qg => qg.MetricKey == metric2.Key.Value);

        var singleQuotaDTOs = quotaGroupDTOs.SelectMany(group => group.Quotas).ToList();

        singleQuotaDTOs.Should().HaveCount(3);

        singleQuotaDTOs.Where(sq => sq.MetricKey == metric1.Key.Value).Should().HaveCount(2);
        singleQuotaDTOs.Where(sq => sq.MetricKey == metric2.Key.Value).Should().HaveCount(1);

        var tierQuota = singleQuotaDTOs.Single(q => q.Source == QuotaSource.Tier && q.MetricKey == metric1.Key.Value);
        var individualQuotaTokens = singleQuotaDTOs.Single(q => q.Source == QuotaSource.Individual && q.MetricKey == metric2.Key.Value);
        var individualQuotaMessages = singleQuotaDTOs.Single(q => q.Source == QuotaSource.Individual && q.MetricKey == metric1.Key.Value);

        tierQuota.Max.Should().Be(5);
        tierQuota.Usage.Should().Be(1);
        tierQuota.Period.Should().Be("Month");

        individualQuotaTokens.Max.Should().Be(5);
        individualQuotaTokens.Usage.Should().Be(2);
        individualQuotaTokens.Period.Should().Be("Month");

        individualQuotaMessages.Max.Should().Be(10);
        individualQuotaMessages.Usage.Should().Be(1);
        individualQuotaMessages.Period.Should().Be("Week");
    }
}
