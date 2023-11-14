using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Quotas.Application.Identities.Queries.GetQuotasForIdentity;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.Metrics;
using FakeItEasy;
using FluentAssertions;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities.GetQuotasForIdentity;
public class HandlerTests
{
    [Fact]
    public async Task Returns_individual_and_tier_quotas_for_identity()
    {
        // Arrange
        var metric = new Metric(MetricKey.NumberOfSentMessages, "Number Of Sent Messages");
        var identity = new Identity(CreateRandomIdentityAddress(), new TierId("SomeTierId"));

        var max = 5;
        var period = QuotaPeriod.Month;

        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(metric.Key, max, period));
        identity.CreateIndividualQuota(metric.Key, max, period);

        var dummyMetricCalculationFactory = A.Fake<MetricCalculatorFactory>();

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(identity.Address);

        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => fakeIdentitiesRepository.Find(identity.Address, A<CancellationToken>._, A<bool>._)).Returns(identity);

        var handler = new Handler(fakeUserContext, fakeIdentitiesRepository, dummyMetricCalculationFactory);

        // Act
        var quotas = (await handler.Handle(new ListQuotasForIdentityQuery(), CancellationToken.None)).Items.ToList();

        // Assert
        quotas.Should().HaveCount(2);
        
        quotas[0].Id.Should().Be(identity.IndividualQuotas.First().Id);
        quotas[0].Max.Should().Be(max);
        quotas[0].Period.Should().Be(period.ToString());
        quotas[0].Source.Should().Be(QuotaSource.Individual);

        quotas[1].Id.Should().Be(identity.TierQuotas.First().Id);
        quotas[1].Max.Should().Be(max);
        quotas[1].Period.Should().Be(period.ToString());
        quotas[1].Source.Should().Be(QuotaSource.Tier);
    }
}
