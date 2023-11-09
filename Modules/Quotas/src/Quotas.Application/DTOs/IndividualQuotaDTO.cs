using System.Threading;
using AutoMapper;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Google.Api.Gax;

namespace Backbone.Modules.Quotas.Application.DTOs;

public class IndividualQuotaDTO : IHaveCustomMapping
{
    public IndividualQuotaDTO() { }

    public IndividualQuotaDTO(string id, MetricDTO metric, int max, QuotaPeriod period)
    {
        Id = id;
        Metric = metric;
        Max = max;
        Period = period;
    }

    public string Id { get; set; }
    public MetricDTO Metric { get; set; }
    public int Max { get; set; }
    public QuotaPeriod Period { get; set; }

    public void CreateMappings(Profile configuration)
    {
        configuration
            .CreateMap<IndividualQuota, IndividualQuotaDTO>()
            .ForMember(dto => dto.Id,
                expression => expression.MapFrom(quota => quota.Id.ToString()))
            .ForMember(dto => dto.Metric,
                expression => expression.MapFrom<MetricToMetricDtoResolver>())
            .ForMember(dto => dto.Max,
                expression => expression.MapFrom(quota => quota.Max))
            .ForMember(dto => dto.Period,
                expression => expression.MapFrom(quota => quota.Period));
    }
}

public class MetricToMetricDtoResolver : IValueResolver<IndividualQuota, IndividualQuotaDTO, MetricDTO>
{
    private readonly IMetricsRepository _metricRepository;

    public MetricToMetricDtoResolver(IMetricsRepository metricRepository)
    {
        _metricRepository = metricRepository;
    }

    public MetricDTO Resolve(IndividualQuota source, IndividualQuotaDTO destination, MetricDTO destMember, ResolutionContext context)
    {
        var metric = _metricRepository.FindAllWithKeys(new List<MetricKey> { source.MetricKey }, CancellationToken.None);
        return new MetricDTO(metric.ResultWithUnwrappedExceptions().First()); // todo: dirty?
    }
}
