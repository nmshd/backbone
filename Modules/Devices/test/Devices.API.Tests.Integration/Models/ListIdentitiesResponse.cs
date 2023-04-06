using Newtonsoft.Json;

namespace Devices.API.Tests.Integration.Models;
public class ListIdentitiesResponse
{
    public List<IdentitySummaryDTO>? Result { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Error? Error { get; set; }
}
