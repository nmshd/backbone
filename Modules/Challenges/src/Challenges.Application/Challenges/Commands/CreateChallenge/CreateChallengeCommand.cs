using Challenges.Application.Challenges.DTOs;
using MediatR;

namespace Challenges.Application.Challenges.Commands.CreateChallenge;

public class CreateChallengeCommand : IRequest<ChallengeDTO> { }
