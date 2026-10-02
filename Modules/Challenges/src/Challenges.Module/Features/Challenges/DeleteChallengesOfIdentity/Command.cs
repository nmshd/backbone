using MediatR;

namespace Backbone.Modules.Challenges.Module.Features.Challenges.DeleteChallengesOfIdentity;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("DeleteChallengesOfIdentityCommand")]
public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
