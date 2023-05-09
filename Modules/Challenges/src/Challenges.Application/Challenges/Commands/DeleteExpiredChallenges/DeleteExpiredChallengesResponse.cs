namespace Backbone.Modules.Challenges.Application.Challenges.Commands.DeleteExpiredChallenges;

public class DeleteExpiredChallengesResponse
{
    public DeleteExpiredChallengesResponse(int deletedChallenges)
    {
        DeletedChallenges = deletedChallenges;
    }

    public int DeletedChallenges { get; set; }

    public static DeleteExpiredChallengesResponse NoDeletedChallenges()
    {
        return new DeleteExpiredChallengesResponse(0);
    }
}
