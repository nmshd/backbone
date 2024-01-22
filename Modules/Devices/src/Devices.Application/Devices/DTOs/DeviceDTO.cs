using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities;

namespace Backbone.Modules.Devices.Application.Devices.DTOs;

public class DeviceDTO : IHaveCustomMapping
{
    public DeviceId? Id { get; set; }

    public string? Username { get; set; }

    public DateTime CreatedAt { get; set; }

    public DeviceId? CreatedByDevice { get; set; }

    public LastLoginInformation? LastLogin { get; set; }

    public void CreateMappings(Profile configuration)
    {
        configuration
            .CreateMap<Device, DeviceDTO>()
            .ForMember(dto => dto.LastLogin,
                expression => expression.MapFrom(device => new LastLoginInformation { Time = device.User.LastLoginAt }))
            .ForMember(dto => dto.Username,
                expression => expression.MapFrom(device => device.User.UserName));
    }
}

public class LastLoginInformation
{
    public DateTime? Time { get; set; }
}
