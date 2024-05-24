using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Messages.Domain.Entities;

namespace Backbone.Modules.Messages.Application.Messages.DTOs;

public class AttachmentDTO : IHaveCustomMapping
{
    public required string Id { get; set; }

    public void CreateMappings(Profile configuration)
    {
        configuration.CreateMap<Attachment, AttachmentDTO>()
            .ForMember(dto => dto.Id, expression => expression.MapFrom(e => e.Id.Value));
    }
}
