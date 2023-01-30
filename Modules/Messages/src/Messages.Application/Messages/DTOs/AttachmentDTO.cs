using Backbone.Modules.Messages.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;

namespace Backbone.Modules.Messages.Application.Messages.DTOs;

public class AttachmentDTO : IMapTo<Attachment>
{
    public string Id { get; set; }
}
