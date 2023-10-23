using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Devices.Domain.Entities;

namespace Backbone.Devices.Application.Clients.DTOs;

public class ClientDTO : IHaveCustomMapping
{
    public string ClientId { get; set; }
    public string DisplayName { get; set; }
    public string DefaultTier { get; set; }
    public DateTime CreatedAt { get; set; }

    public void CreateMappings(Profile configuration)
    {
        configuration.CreateMap<OAuthClient, ClientDTO>()
            .ForMember(dto => dto.DefaultTier, expression => expression.MapFrom(m => m.DefaultTier.Value));
    }
}
