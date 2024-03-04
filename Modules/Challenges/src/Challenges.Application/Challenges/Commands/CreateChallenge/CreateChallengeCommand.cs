using Backbone.Modules.Challenges.Application.Challenges.DTOs;
using MediatR;

namespace Backbone.Modules.Challenges.Application.Challenges.Commands.CreateChallenge;

public class CreateChallengeCommand : IRequest<ChallengeDTO>;
