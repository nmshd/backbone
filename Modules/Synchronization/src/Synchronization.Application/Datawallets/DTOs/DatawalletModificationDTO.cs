using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Domain.Entities;

namespace Backbone.Modules.Synchronization.Application.Datawallets.DTOs;

public class DatawalletModificationDTO : IHaveCustomMapping
{
    public enum DatawalletModificationType
    {
        Create,
        Update,
        Delete,
        CacheChanged
    }

    public required DatawalletModificationId Id { get; set; }
    public required ushort DatawalletVersion { get; set; }
    public required long Index { get; set; }
    public required string ObjectIdentifier { get; set; }
    public string? PayloadCategory { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DeviceId CreatedByDevice { get; set; }
    public required string Collection { get; set; }
    public required string Type { get; set; }
    public byte[]? EncryptedPayload { get; set; }


    public void CreateMappings(Profile configuration)
    {
        configuration.CreateMap<DatawalletModification, DatawalletModificationDTO>();

        configuration.CreateMap<Datawallet.DatawalletVersion, ushort>().ConvertUsing(version => version.Value);

        configuration.CreateMap<Domain.Entities.DatawalletModificationType, DatawalletModificationType>().ConvertUsingEnumMapping(
            config => config
                .MapValue(Domain.Entities.DatawalletModificationType.CacheChanged, DatawalletModificationType.CacheChanged)
                .MapValue(Domain.Entities.DatawalletModificationType.Create, DatawalletModificationType.Create)
                .MapValue(Domain.Entities.DatawalletModificationType.Delete, DatawalletModificationType.Delete)
                .MapValue(Domain.Entities.DatawalletModificationType.Update, DatawalletModificationType.Update));
    }
}
