using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Devices.Domain.Entities;

namespace Backbone.Modules.Devices.Application.Clients.DTOs;
public class ClientOverviewDTO : IHaveCustomMapping
{
    public required string ClientId { get; set; }
    public required string DisplayName { get; set; }

    public void CreateMappings(Profile configuration)
    {
        configuration.CreateMap<OAuthClient, ClientOverviewDTO>();
    }
}
