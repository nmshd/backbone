namespace Backbone.Modules.Challenges.Application.Challenges.Commands.DeleteExpiredChallenges;

public class DeleteExpiredChallengesResponse
{
    public DeleteExpiredChallengesResponse(int deletedChallenges)
    {
        NumberOfDeletedChallenges = deletedChallenges;
    }

    public int NumberOfDeletedChallenges { get; set; }

    public static DeleteExpiredChallengesResponse NoDeletedChallenges()
    {
        return new DeleteExpiredChallengesResponse(0);
    }
}
