using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Challenges.Domain.Ids;
using FluentValidation;

namespace Backbone.Modules.Challenges.Module.Features.Challenges.GetChallengeById;

public class Validator : AbstractValidator<GetChallengeByIdQuery>
{
    public Validator()
    {
        RuleFor(x => x.Id).ValidId<GetChallengeByIdQuery, ChallengeId>();
    }
}
