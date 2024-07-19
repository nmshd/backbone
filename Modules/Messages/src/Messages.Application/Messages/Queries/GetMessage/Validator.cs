using Backbone.Modules.Messages.Domain.Ids;
using FluentValidation;

namespace Backbone.Modules.Messages.Application.Messages.Queries.GetMessage;

public class Validator : AbstractValidator<GetMessageQuery>
{
    public Validator()
    {
        RuleFor(x => x.Id).Must(MessageId.IsValid);
    }
}
