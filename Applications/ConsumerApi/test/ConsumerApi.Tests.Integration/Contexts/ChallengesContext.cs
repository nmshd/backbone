using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;

namespace Backbone.ConsumerApi.Tests.Integration.Contexts;

public class ChallengesContext
{
    public readonly Dictionary<string, Challenge> Challenges = new();
}
