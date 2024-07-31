using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.Modules.Tokens.Application.Tokens.DTOs;

public class TokenDTO : IHaveCustomMapping
{
    public required string Id { get; set; }

    public required string CreatedBy { get; set; }
    public required string CreatedByDevice { get; set; }

    public required DateTime CreatedAt { get; set; }
    public required DateTime ExpiresAt { get; set; }

    public required byte[] Content { get; set; }

    public void CreateMappings(Profile configuration)
    {
        configuration.CreateMap<Token, TokenDTO>()
            .ForMember(dto => dto.Id, expression => expression.MapFrom(t => t.Id.Value))
            .ForMember(dto => dto.CreatedBy, expression => expression.MapFrom(t => t.CreatedBy.Value))
            .ForMember(dto => dto.CreatedByDevice, expression => expression.MapFrom(t => t.CreatedByDevice.Value));
    }
}
