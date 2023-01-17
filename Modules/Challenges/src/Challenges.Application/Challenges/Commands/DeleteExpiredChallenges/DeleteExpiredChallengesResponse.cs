using Challenges.Domain.Ids;

namespace Challenges.Application.Challenges.Commands.DeleteExpiredChallenges;

public class DeleteExpiredChallengesResponse
{
    public DeleteExpiredChallengesResponse(IEnumerable<ChallengeId> deletedChallenges)
    {
        DeletedChallenges = deletedChallenges;
    }

    public IEnumerable<ChallengeId> DeletedChallenges { get; set; }

    public static DeleteExpiredChallengesResponse NoDeletedChallenges()
    {
        return new DeleteExpiredChallengesResponse(Array.Empty<ChallengeId>());
    }
}
