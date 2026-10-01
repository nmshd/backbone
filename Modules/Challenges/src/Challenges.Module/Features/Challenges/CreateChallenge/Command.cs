using Backbone.Modules.Challenges.Module.Features.Challenges.Shared;
using MediatR;

namespace Backbone.Modules.Challenges.Module.Features.Challenges.CreateChallenge;

public class CreateChallengeCommand : IRequest<ChallengeDTO>;
