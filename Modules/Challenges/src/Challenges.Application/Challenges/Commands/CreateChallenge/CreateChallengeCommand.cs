using Backbone.Challenges.Application.Challenges.DTOs;
using MediatR;

namespace Backbone.Challenges.Application.Challenges.Commands.CreateChallenge;

public class CreateChallengeCommand : IRequest<ChallengeDTO> { }
