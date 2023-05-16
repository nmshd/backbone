using AutoMapper;
using Backbone.Modules.Files.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Application.Files.Commands.CreateFile;

public class CreateFileResponse : IHaveCustomMapping
{
    public FileId Id { get; set; }

    public DateTime CreatedAt { get; set; }
    public IdentityAddress CreatedBy { get; set; }
    public DeviceId CreatedByDevice { get; set; }

    public DateTime ModifiedAt { get; set; }
    public IdentityAddress ModifiedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public IdentityAddress DeletedBy { get; set; }

    public IdentityAddress Owner { get; set; }
    public byte[] OwnerSignature { get; set; }

    public long CipherSize { get; set; }
    public byte[] CipherHash { get; set; }

    public DateTime ExpiresAt { get; set; }

    public void CreateMappings(Profile configuration)
    {
        configuration
            .CreateMap<File, CreateFileResponse>()
            .ForMember(dto => dto.OwnerSignature, c => c.MapFrom(f => f.OwnerSignature.Length == 0 ? null : f.OwnerSignature));
    }
}
