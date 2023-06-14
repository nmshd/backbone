using MediatR;

namespace Backbone.Modules.Quotas.Application.Metrics.Commands.RecalculateMetricStatuses;
public class RecalculateMetricStatusesCommand : IRequest
{
    public RecalculateMetricStatusesCommand(List<string> identities, List<string> metrics)
    {
        Identities = identities;
        Metrics = metrics;
    }

    public List<string> Identities { get; set; }
    public List<string> Metrics { get; set; }
}
