using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.Tooling.Extensions;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Messages.Application.Messages.Commands.SendMessage;

public class Validator : AbstractValidator<SendMessageCommand>
{
    public Validator(IOptions<ApplicationConfiguration> options)
    {
        RuleFor(m => m.Recipients)
            .DetailedNotNull()
            .UniqueItems(r => r.Address).WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code)
            .ForEach(r => r
                .DetailedNotNull()
                .WithName("Recipient")
                .SetValidator(new SendMessageCommandRecipientInformationValidator()));

        RuleFor(m => m.Recipients.Count)
            .InclusiveBetween(1, options.Value.MaxNumberOfMessageRecipients)
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);

        RuleFor(m => m.Body).DetailedNotNull().NumberOfBytes(1, 10.Mebibytes());

        RuleFor(m => m.Attachments)
            .ForEach(i => i
                .DetailedNotNull()
                .SetValidator(new SendMessageCommandAttachmentValidator()));

        RuleFor(m => m.Attachments.Count)
            .InclusiveBetween(0, 20).WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
    }
}

public class SendMessageCommandRecipientInformationValidator : AbstractValidator<SendMessageCommandRecipientInformation>
{
    public SendMessageCommandRecipientInformationValidator()
    {
        RuleFor(r => r.Address).ValidId<SendMessageCommandRecipientInformation, IdentityAddress>();

        RuleFor(r => r.EncryptedKey)
            .DetailedNotNull()
            .NumberOfBytes(30, 300);
    }
}

public class SendMessageCommandAttachmentValidator : AbstractValidator<SendMessageCommandAttachment>
{
    public SendMessageCommandAttachmentValidator()
    {
        RuleFor(a => a.Id)
            .DetailedNotNull()
            .Must(FileId.IsValid).WithMessage("{PropertyName} has an invalid format.");
    }
}
