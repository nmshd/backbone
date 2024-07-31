using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

namespace Backbone.Modules.Relationships.Application.Relationships.DTOs;

public class RelationshipTemplateDTO : IMapTo<RelationshipTemplate>
{
    public required string Id { get; set; }

    public required string CreatedBy { get; set; }
    public required string CreatedByDevice { get; set; }
    public int? MaxNumberOfAllocations { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public byte[]? Content { get; set; }

    public required DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public void CreateMappings(Profile configuration)
    {
        configuration.CreateMap<RelationshipTemplate, RelationshipTemplateDTO>()
            .ForMember(dto => dto.Id, expression => expression.MapFrom(t => t.Id.Value))
            .ForMember(dto => dto.CreatedBy, expression => expression.MapFrom(t => t.CreatedBy.Value))
            .ForMember(dto => dto.CreatedByDevice, expression => expression.MapFrom(t => t.CreatedByDevice.Value));
    }
}
