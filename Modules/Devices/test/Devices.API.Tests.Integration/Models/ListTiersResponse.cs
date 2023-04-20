using Newtonsoft.Json;

namespace Devices.API.Tests.Integration.Models;
public class ListTiersResponse
{
    public List<TierDTO>? Result { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Error? Error { get; set; }
}
