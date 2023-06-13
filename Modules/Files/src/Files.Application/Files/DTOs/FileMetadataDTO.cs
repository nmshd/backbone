using AutoMapper;
using Backbone.Modules.Files.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Application.Files.DTOs;

public class FileMetadataDTO : IHaveCustomMapping
{
    public FileId Id { get; set; }

    public DateTime CreatedAt { get; set; }
    public IdentityAddress CreatedBy { get; set; }
    public DeviceId CreatedByDevice { get; set; }

    public DateTime ModifiedAt { get; set; }
    public IdentityAddress ModifiedBy { get; set; }
    public DeviceId ModifiedByDevice { get; set; }

    public DateTime? DeletedAt { get; set; }
    public IdentityAddress DeletedBy { get; set; }
    public DeviceId DeletedByDevice { get; set; }

    public IdentityAddress Owner { get; set; }
    public byte[] OwnerSignature { get; set; }

    public long CipherSize { get; set; }
    public byte[] CipherHash { get; set; }

    public DateTime ExpiresAt { get; set; }

    public byte[] EncryptedProperties { get; set; }

    public void CreateMappings(Profile configuration)
    {
        configuration
            .CreateMap<File, FileMetadataDTO>()
            .ForMember(dto => dto.OwnerSignature, c => c.MapFrom(f => f.OwnerSignature.Length == 0 ? null : f.OwnerSignature));
    }
}
