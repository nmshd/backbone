using Backbone.BuildingBlocks.Application.Attributes;
using MediatR;

namespace Backbone.Modules.Messages.Module.Features.Messages.SendMessage;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("SendMessageCommand")]
[ApplyQuotasForMetrics("NumberOfSentMessages")]
public class Command : IRequest<Response>
{
    public ICollection<SendMessageCommandRecipientInformation> Recipients { get; set; } = new List<SendMessageCommandRecipientInformation>();
    public required byte[] Body { get; set; }
    public ICollection<SendMessageCommandAttachment> Attachments { get; set; } = new List<SendMessageCommandAttachment>();
}

public class SendMessageCommandRecipientInformation
{
    public required string Address { get; set; }
    public required byte[] EncryptedKey { get; set; }
}

public class SendMessageCommandAttachment
{
    public required string Id { get; set; }
}
