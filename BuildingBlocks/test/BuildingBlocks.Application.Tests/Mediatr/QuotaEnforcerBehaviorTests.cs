using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.BuildingBlocks.Application.Attributes;
using Enmeshed.BuildingBlocks.Domain;
using MediatR;

namespace Enmeshed.BuildingBlocks.Application.Tests.Mediatr;
public class QuotaEnforcerBehaviorTests
{
    private readonly IUserContext _userContextStub;

    public QuotaEnforcerBehaviorTests()
    {
        _userContextStub = new UserContextStub();
    }

    /// <summary>
    /// The Metric key doesn't matter for these tests. The way the Mediatr Behavior is being
    /// called does not inject the Metrics passed on the Attribute below. Tests will make use of
    /// all the metrics available in the repository unless where specified.
    /// </summary>
    [ApplyQuotasForMetrics("DoesNotApplyToTests")]
    private class TestCommand : IRequest { }

    private class IResponse { }

}

public static class TestData
{
    public static class MetricStatus
    {
        public static readonly Domain.MetricStatus ThatIsExhaustedFor1Day = new(new MetricKey("ExhaustedUntilTomorrow"), DateTime.Now.AddDays(1));

        public static readonly Domain.MetricStatus ThatIsExhaustedFor10Days = new(new MetricKey("ExhaustedFor10Days"), DateTime.Now.AddDays(10));

        public static readonly Domain.MetricStatus ThatWasExhaustedUntilYesterday = new(new MetricKey("ExhaustedUntilYesterday"), DateTime.Now.AddDays(-1));

        public static readonly Domain.MetricStatus ThatIsNotExhausted = new(new MetricKey("ExhaustedUntilNull"), null);
    }
}
