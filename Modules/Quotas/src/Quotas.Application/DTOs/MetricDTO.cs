using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Application.DTOs;

public class MetricDTO : IHaveCustomMapping
{
    public MetricDTO(Metric metric)
    {
        Key = metric.Key.Value;
        DisplayName = metric.DisplayName;
    }

    public string Key { get; set; }
    public string DisplayName { get; set; }

    public void CreateMappings(Profile configuration)
    {
        configuration.CreateMap<Metric, MetricDTO>()
            .ForMember(dto => dto.Key, expression => expression.MapFrom(m => m.Key.Value));
    }
}
