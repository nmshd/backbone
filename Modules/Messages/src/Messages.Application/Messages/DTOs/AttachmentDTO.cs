using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Messages.Domain.Entities;

namespace Backbone.Modules.Messages.Application.Messages.DTOs;

public class AttachmentDTO : IMapTo<Attachment>
{
    public string? Id { get; set; }
}
