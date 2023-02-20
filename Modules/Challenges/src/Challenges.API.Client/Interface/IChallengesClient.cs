using Challenges.API.Client.Models;

namespace Challenges.API.Client.Interface;
internal interface IChallengesClient
{
    Task<ChallengeResponse> CreateChallenge(AuthenticationRequest authenticationRequest);

    Task<ChallengeResponse> GetChallengeById(string challengeId, AuthenticationRequest authenticationRequest);
}
