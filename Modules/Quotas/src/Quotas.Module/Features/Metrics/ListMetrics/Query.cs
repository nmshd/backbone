using MediatR;

namespace Backbone.Modules.Quotas.Module.Features.Metrics.ListMetrics;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListMetricsQuery")]
public class Query : IRequest<Response>;
