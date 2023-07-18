using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.Tests.TestDoubles;
using Backbone.Modules.Quotas.Application.Tiers.Commands.CreateQuotaForIdentity;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Quotas.CreateQuotaForIdentity;

public class HandlerTests
{
    [Fact]
    public async Task Creates_quota_for_identity()
    {
        // Arrange
        var max = 5;
        var period = QuotaPeriod.Month;
        var metricKey = MetricKey.NumberOfSentMessages.Value;
        var identityAddress = IdentityAddress.Parse("id1KJnD8ipfckRQ1ivAhNVLtypmcVM5vPX4j");
        var tierId = new TierId("TIRsomeTierId1111111");
        var command = new CreateQuotaForIdentityCommand(identityAddress, metricKey, max, period);
        var identity = new Identity("id1KJnD8ipfckRQ1ivAhNVLtypmcVM5vPX4j", tierId);

        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.FindByAddress(identityAddress, A<CancellationToken>._, A<bool>._)).Returns(identity);

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
                t.Address == identityAddress &&
                t.TierId == tierId &&
                t.IndividualQuotas.Count == 1)
            , CancellationToken.None)
        ).MustHaveHappened();
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, IMetricsRepository metricsRepository)
    {
        var logger = A.Fake<ILogger<Handler>>();
        return new Handler(identitiesRepository, logger, metricsRepository);
    }
}
