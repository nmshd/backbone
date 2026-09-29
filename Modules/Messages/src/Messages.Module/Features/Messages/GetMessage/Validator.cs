using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Messages.Domain.Ids;
using FluentValidation;

namespace Backbone.Modules.Messages.Module.Features.Messages.GetMessage;

public class Validator : AbstractValidator<GetMessageQuery>
{
    public Validator()
    {
        RuleFor(query => query.Id).ValidId<GetMessageQuery, MessageId>();
    }
}
