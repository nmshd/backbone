using Newtonsoft.Json;

namespace ConsumerApi.Tests.Integration.Models;

public class ChallengeResponse
{
    public Challenge? Result { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Error? Error { get; set; }
}
