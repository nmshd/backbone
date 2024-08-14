using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Messages.Domain.Ids;
using FluentValidation;

namespace Backbone.Modules.Messages.Application.Messages.Queries.ListMessages;

// ReSharper disable once UnusedMember.Global
public class Validator : AbstractValidator<ListMessagesQuery>
{
    public Validator()
    {
        RuleForEach(x => x.Ids).ValidId<ListMessagesQuery, MessageId>();
    }
}
