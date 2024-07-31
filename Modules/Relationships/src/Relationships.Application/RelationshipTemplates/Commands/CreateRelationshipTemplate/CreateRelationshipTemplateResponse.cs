using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.CreateRelationshipTemplate;

public class CreateRelationshipTemplateResponse : IHaveCustomMapping
{
    public required string Id { get; set; }
    public required DateTime CreatedAt { get; set; }

    public void CreateMappings(Profile configuration)
    {
        configuration.CreateMap<RelationshipTemplate, CreateRelationshipTemplateResponse>()
            .ForMember(dto => dto.Id, expression => expression.MapFrom(m => m.Id.Value));
    }
}
