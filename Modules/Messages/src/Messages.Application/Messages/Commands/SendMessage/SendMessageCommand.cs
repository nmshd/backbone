using Backbone.BuildingBlocks.Application.Attributes;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Messages.Application.Messages.Commands.SendMessage;

[ApplyQuotasForMetrics("NumberOfSentMessages")]
public class SendMessageCommand : IRequest<SendMessageResponse>
{
    public ICollection<SendMessageCommandRecipientInformation> Recipients { get; set; } = new List<SendMessageCommandRecipientInformation>();
    public DateTime? DoNotSendBefore { get; set; }
    public byte[] Body { get; set; }
    public ICollection<SendMessageCommandAttachment> Attachments { get; set; } = new List<SendMessageCommandAttachment>();
}

public class SendMessageCommandRecipientInformation
{
    public IdentityAddress? Address { get; set; }
    public byte[] EncryptedKey { get; set; }
}

public class SendMessageCommandAttachment
{
    public string Id { get; set; }
}
