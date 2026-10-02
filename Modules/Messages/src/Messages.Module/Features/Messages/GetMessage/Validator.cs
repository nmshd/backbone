using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Messages.Domain.Ids;
using FluentValidation;

namespace Backbone.Modules.Messages.Module.Features.Messages.GetMessage;

public class Validator : AbstractValidator<Query>
{
    public Validator()
    {
        RuleFor(query => query.Id).ValidId<Query, MessageId>();
    }
}
