using Devices.API.Tests.Integration.Models;
using Newtonsoft.Json;

namespace Challenges.API.Tests.Integration.Models;
public class IdentityResponse
{
    public List<IdentitySummaryDTO>? Result { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Error? Error { get; set; }
}
