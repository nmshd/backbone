using MediatR;

namespace Challenges.Application.Challenges.Commands.DeleteExpiredChallenges;

public class DeleteExpiredChallengesCommand : IRequest<DeleteExpiredChallengesResponse> { }
