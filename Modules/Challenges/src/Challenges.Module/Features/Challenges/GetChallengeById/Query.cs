using Backbone.Modules.Challenges.Module.Features.Challenges.Shared;
using MediatR;

namespace Backbone.Modules.Challenges.Module.Features.Challenges.GetChallengeById;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("GetChallengeByIdQuery")]
public class Query : IRequest<ChallengeDTO>
{
    public required string Id { get; init; }
}
