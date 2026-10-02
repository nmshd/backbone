using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.Tooling.Extensions;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Messages.Module.Features.Messages.SendMessage;

public class Validator : AbstractValidator<Command>
{
    public Validator(IOptions<ApplicationConfiguration> options)
    {
        RuleFor(message => message.Recipients)
            .DetailedNotNull()
            .UniqueItems(recipient => recipient.Address).WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code)
            .ForEach(recipient => recipient.DetailedNotNull().WithName("Recipient").SetValidator(new RecipientValidator()));

        RuleFor(message => message.Recipients.Count)
            .InclusiveBetween(1, options.Value.MaxNumberOfMessageRecipients)
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
        RuleFor(message => message.Body).DetailedNotNull().NumberOfBytes(1, 10.Mebibytes());
        RuleFor(message => message.Attachments).ForEach(attachment => attachment.DetailedNotNull().SetValidator(new AttachmentValidator()));
        RuleFor(message => message.Attachments.Count).InclusiveBetween(0, 20).WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
    }
}

public class RecipientValidator : AbstractValidator<SendMessageCommandRecipientInformation>
{
    public RecipientValidator()
    {
        RuleFor(recipient => recipient.Address).ValidId<SendMessageCommandRecipientInformation, IdentityAddress>();
        RuleFor(recipient => recipient.EncryptedKey).DetailedNotNull().NumberOfBytes(30, 300);
    }
}

public class AttachmentValidator : AbstractValidator<SendMessageCommandAttachment>
{
    public AttachmentValidator()
    {
        RuleFor(attachment => attachment.Id).DetailedNotNull().Must(FileId.IsValid).WithMessage("{PropertyName} has an invalid format.");
    }
}
