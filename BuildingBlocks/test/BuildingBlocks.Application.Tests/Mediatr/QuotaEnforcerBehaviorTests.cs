using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.BuildingBlocks.Application.Attributes;
using Enmeshed.BuildingBlocks.Domain;
using MediatR;
using DomainMetricStatus = Enmeshed.BuildingBlocks.Domain.MetricStatus;

namespace Enmeshed.BuildingBlocks.Application.Tests.Mediatr;
public class QuotaEnforcerBehaviorTests
{
    private readonly IUserContext _userContextStub;

    public QuotaEnforcerBehaviorTests()
    {
        _userContextStub = new UserContextStub();
    }

    [ApplyQuotasForMetrics("*")]
    private class TestCommand : IRequest { }

    private class IResponse { }

}

public static class TestData
{
    public static class MetricStatus
    {
        public static readonly DomainMetricStatus ThatIsExhaustedFor1Day = new(new MetricKey("ExhaustedUntilTomorrow"), DateTime.Now.AddDays(1));

        public static readonly DomainMetricStatus ThatIsExhaustedFor10Days = new(new MetricKey("ExhaustedFor10Days"), DateTime.Now.AddDays(10));

        public static readonly DomainMetricStatus ThatWasExhaustedUntilYesterday = new(new MetricKey("ExhaustedUntilYesterday"), DateTime.Now.AddDays(-1));

        public static readonly DomainMetricStatus ThatIsNotExhausted = new(new MetricKey("ExhaustedUntilNull"), null);
    }
}
