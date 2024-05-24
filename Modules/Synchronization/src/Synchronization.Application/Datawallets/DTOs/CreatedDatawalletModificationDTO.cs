using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Synchronization.Domain.Entities;

namespace Backbone.Modules.Synchronization.Application.Datawallets.DTOs;

public class CreatedDatawalletModificationDTO : IHaveCustomMapping
{
    public required string Id { get; set; }
    public required long Index { get; set; }
    public required DateTime CreatedAt { get; set; }

    public void CreateMappings(Profile configuration)
    {
        configuration.CreateMap<DatawalletModification, CreatedDatawalletModificationDTO>()
            .ForMember(dto => dto.Id, expression => expression.MapFrom(d => d.Id.Value));
    }
}
