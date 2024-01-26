using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Files.Domain.Entities;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Application.Files.Commands.CreateFile;

public class CreateFileResponse : IHaveCustomMapping
{
    public required FileId Id { get; set; }

    public required DateTime CreatedAt { get; set; }
    public required IdentityAddress CreatedBy { get; set; }
    public required DeviceId CreatedByDevice { get; set; }

    public required DateTime ModifiedAt { get; set; }
    public required IdentityAddress ModifiedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public IdentityAddress? DeletedBy { get; set; }

    public required IdentityAddress Owner { get; set; }
    public required byte[] OwnerSignature { get; set; }

    public required long CipherSize { get; set; }
    public required byte[] CipherHash { get; set; }

    public required DateTime ExpiresAt { get; set; }

    public void CreateMappings(Profile configuration)
    {
        configuration
            .CreateMap<File, CreateFileResponse>()
            .ForMember(dto => dto.OwnerSignature, c => c.MapFrom(f => f.OwnerSignature.Length == 0 ? null : f.OwnerSignature));
    }
}
