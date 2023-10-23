using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Messages.Domain.Entities;

namespace Backbone.Messages.Application.Messages.DTOs;

public class AttachmentDTO : IMapTo<Attachment>
{
    public string Id { get; set; }
}
