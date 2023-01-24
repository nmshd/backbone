using AutoMapper;
using Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Devices.Application.Devices.DTOs;

public class DeviceDTO : IHaveCustomMapping
{
    public DeviceId Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DeviceId CreatedByDevice { get; set; }

    public DateTime? DeletedAt { get; set; }
    public DeviceId DeletedByDevice { get; set; }
    public byte[] DeletionCertificate { get; set; }

    public LastLoginInformation LastLogin { get; set; }

    public void CreateMappings(Profile configuration)
    {
        configuration
            .CreateMap<Device, DeviceDTO>()
            .ForMember(dto => dto.LastLogin,
                expression => expression.MapFrom(device => new LastLoginInformation {Time = device.User.LastLoginAt}));
    }
}

public class LastLoginInformation
{
    public DateTime? Time { get; set; }
}
