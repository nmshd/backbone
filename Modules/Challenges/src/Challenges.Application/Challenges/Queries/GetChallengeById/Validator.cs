using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Challenges.Domain.Ids;
using FluentValidation;

namespace Backbone.Modules.Challenges.Application.Challenges.Queries.GetChallengeById;

public class Validator : AbstractValidator<GetChallengeByIdQuery>
{
    public Validator()
    {
        RuleFor(x => x.Id).ValidId<GetChallengeByIdQuery, ChallengeId>();
    }
}
