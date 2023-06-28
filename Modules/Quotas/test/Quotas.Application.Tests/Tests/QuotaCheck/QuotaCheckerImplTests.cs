using Backbone.Modules.Quotas.Application.QuotaCheck;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.Common.Infrastructure.Persistence.Repository;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;
using FluentAssertions;
using MediatR;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.QuotaCheck;
public class QuotaCheckerImplTests
{
    [Fact]
    public async Task ExhaustedStatuses_is_empty_when_the_metric_is_not_exhausted()
    {
        // Arrange
        var quotaChecker = CreateQuotaCheckerImplementation(new MetricStatus(TestMetricKey, null));

        // Act
        var result = await quotaChecker.CheckQuotaExhaustion(new[] { TestMetricKey });

        // Assert
        result.ExhaustedStatuses.Should().HaveCount(0);
    }

    [Fact]
    public async Task ExhaustedStatuses_has_value_when_one_metric_is_exhausted()
    {
        // Arrange
        var quotaChecker = CreateQuotaCheckerImplementation(new MetricStatus(TestMetricKey, SystemTime.UtcNow.AddMinutes(10)));

        // Act
        var result = await quotaChecker.CheckQuotaExhaustion(new[] { TestMetricKey });

        // Assert
        result.ExhaustedStatuses.Should().HaveCount(1);
    }

    [Fact]
    public async Task ExhaustedStatuses_has_values_when_several_metrics_are_exhausted()
    {
        // Arrange
        var quotaChecker = CreateQuotaCheckerImplementation(
            new MetricStatus(TestMetricKey, SystemTime.UtcNow.AddMinutes(10)),
            new MetricStatus(AnotherTestMetricKey, SystemTime.UtcNow.AddMinutes(10))
            );

        // Act
        var result = await quotaChecker.CheckQuotaExhaustion(new[] { TestMetricKey, AnotherTestMetricKey });

        // Assert
        result.ExhaustedStatuses.Should().HaveCount(2);
    }

    private static QuotaCheckerImpl CreateQuotaCheckerImplementation(params MetricStatus[] metricStatuses)
    {
        return new QuotaCheckerImpl(new UserContextStub(), new MetricStatusesStubRepository(metricStatuses.ToList()));
    }

    private static readonly MetricKey TestMetricKey = new("a-metric-key");
    private static readonly MetricKey AnotherTestMetricKey = new("another-metric-key");
}

internal class UserContextStub : IUserContext
{
    public UserContextStub()
    {
    }

    public IdentityAddress GetAddress()
    {
        return IdentityAddress.Create(Convert.FromBase64String("mJGmNbxiVZAPToRuk9O3NvdfsWl6V+7wzIc+/57bU08="), "id1");
    }

    public IdentityAddress GetAddressOrNull()
    {
        throw new NotImplementedException();
    }

    public DeviceId GetDeviceId()
    {
        throw new NotImplementedException();
    }

    public DeviceId GetDeviceIdOrNull()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<string> GetRoles()
    {
        throw new NotImplementedException();
    }

    public SubscriptionPlan GetSubscriptionPlan()
    {
        throw new NotImplementedException();
    }

    public string GetUserId()
    {
        throw new NotImplementedException();
    }

    public string GetUserIdOrNull()
    {
        throw new NotImplementedException();
    }
}

internal static class TestData
{
    internal class TestCommand : IRequest { }

    internal class IResponse { }
}

internal class MetricStatusesStubRepository : IMetricStatusesRepository
{
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public MetricStatusesStubRepository(List<MetricStatus>? metricStatuses)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    {
        if (metricStatuses != null)
        {
            MetricStatuses = metricStatuses;
        }
    }
    public List<MetricStatus> MetricStatuses { get; } = new();

    public Task<IEnumerable<MetricStatus>> GetMetricStatuses(IdentityAddress identity, IEnumerable<MetricKey> keys)
    {
        return Task.FromResult(MetricStatuses.AsEnumerable());
    }
}
