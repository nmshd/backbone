using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Messages.Domain.Entities;

namespace Messages.Application.Messages.DTOs;

public class AttachmentDTO : IMapTo<Attachment>
{
    public string Id { get; set; }
}
