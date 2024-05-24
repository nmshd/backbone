using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;

namespace Backbone.Modules.Messages.Application.Messages.DTOs;

public class AttachmentDTO : IMapTo<Attachment>
{
    public required FileId Id { get; set; }
}
