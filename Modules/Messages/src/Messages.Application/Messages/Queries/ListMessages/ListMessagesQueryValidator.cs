using Backbone.Modules.Messages.Domain.Ids;
using FluentValidation;

namespace Backbone.Modules.Messages.Application.Messages.Queries.ListMessages;

// ReSharper disable once UnusedMember.Global
public class ListMessagesQueryValidator : AbstractValidator<ListMessagesQuery>
{
    public ListMessagesQueryValidator()
    {
        RuleForEach(x => x.Ids).Must(MessageId.IsValid);
    }
}
