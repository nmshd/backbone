using Newtonsoft.Json;

namespace Challenges.API.Tests.Integration.Models;
public class ChallengeResponse
{
    public Challenge? Result { get; set; }
    [JsonIgnore]
    public Error? Error { get; set; }
}
