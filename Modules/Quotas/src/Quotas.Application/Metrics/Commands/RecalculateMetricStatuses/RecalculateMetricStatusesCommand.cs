using MediatR;

namespace Backbone.Modules.Quotas.Application.Metrics.Commands.RecalculateMetricStatuses;
public class RecalculateMetricStatusesCommand : IRequest
{
    public List<string> Identities { get; set; }
    public List<string> Metrics { get; set; }
}
