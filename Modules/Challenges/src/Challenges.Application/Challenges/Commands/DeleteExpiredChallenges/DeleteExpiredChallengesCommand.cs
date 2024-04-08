using MediatR;

namespace Backbone.Modules.Challenges.Application.Challenges.Commands.DeleteExpiredChallenges;

public class DeleteExpiredChallengesCommand : IRequest<DeleteExpiredChallengesResponse>;
