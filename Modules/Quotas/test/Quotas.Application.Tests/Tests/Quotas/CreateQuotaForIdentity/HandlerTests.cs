using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.Tests.TestDoubles;
using Backbone.Modules.Quotas.Application.Tiers.Commands.CreateQuotaForIdentity;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;
using MetricKey = Backbone.Modules.Quotas.Domain.Aggregates.Metrics.MetricKey;

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
        A.CallTo(() => identitiesRepository.Find(identityAddress, A<CancellationToken>._, A<bool>._)).Returns(identity);

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

    [Fact]
    public async Task Create_quota_with_invalid_metric_key_throws_domain_exception()
    {
        // Arrange
        var command = new CreateQuotaForIdentityCommand(IdentityAddress.Parse("id1KJnD8ipfckRQ1ivAhNVLtypmcVM5vPX4j"), "An-Invalid-Metric-Key", 5, QuotaPeriod.Month);
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        var metricsRepository = new FindMetricsStubRepository(new Metric(MetricKey.NumberOfSentMessages, "Number Of Sent Messages"));
        var handler = CreateHandler(identitiesRepository, metricsRepository);

        // Act
        var exception = await Assert.ThrowsAsync<DomainException>(async () => await handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Contain("The given metric key is not supported.");
        exception.Code.Should().Be("error.platform.quotas.unsupportedMetricKey");
    }

    [Fact]
    public async Task Create_quota_for_non_existent_identity_throws_not_found_exception()
    {
        // Arrange
        var command = new CreateQuotaForIdentityCommand(IdentityAddress.Parse("id1KJnD8ipfckRQ1ivAhNVLtypmcVM5vPX4j"), "An-Invalid-Metric-Key", 5, QuotaPeriod.Month);
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.Find(A<string>._, A<CancellationToken>._, A<bool>._)).Returns((Identity)null);
        var metricsRepository = new FindMetricsStubRepository(new Metric(MetricKey.NumberOfSentMessages, "Number Of Sent Messages"));
        var handler = CreateHandler(identitiesRepository, metricsRepository);

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Be("Identity not found. Make sure the ID exists and the record is not expired.");
        exception.Code.Should().Be("error.platform.recordNotFound");
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, IMetricsRepository metricsRepository)
    {
        var logger = A.Fake<ILogger<Handler>>();
        return new Handler(identitiesRepository, logger, metricsRepository);
    }
}
