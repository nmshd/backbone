using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;

namespace Backbone.Modules.Messages.Application.Messages.DTOs;

public class ViewMessageDTO : IMapTo<Message>
{
    public MessageId Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Body { get; set; }
}
