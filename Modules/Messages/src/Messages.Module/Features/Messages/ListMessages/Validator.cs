using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Messages.Domain.Ids;
using FluentValidation;

namespace Backbone.Modules.Messages.Module.Features.Messages.ListMessages;

public class Validator : AbstractValidator<Query>
{
    public Validator()
    {
        RuleForEach(query => query.Ids).ValidId<Query, MessageId>();
    }
}
