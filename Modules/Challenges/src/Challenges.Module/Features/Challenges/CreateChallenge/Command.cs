using Backbone.Modules.Challenges.Module.Features.Challenges.Shared;
using MediatR;

namespace Backbone.Modules.Challenges.Module.Features.Challenges.CreateChallenge;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("CreateChallengeCommand")]
public class Command : IRequest<ChallengeDTO>;
