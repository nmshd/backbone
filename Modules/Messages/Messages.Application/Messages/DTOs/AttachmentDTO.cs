using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Messages.Domain.Entities;
using Messages.Domain.Ids;

namespace Messages.Application.Messages.DTOs;

public class AttachmentDTO : IMapTo<Attachment>
{
    public FileId Id { get; set; }
}
