using MediatR;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.DeleteAnnouncementRecipients;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("DeleteAnnouncementRecipientsCommand")]
public record Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
