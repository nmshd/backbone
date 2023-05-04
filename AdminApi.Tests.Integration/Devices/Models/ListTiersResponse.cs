using Newtonsoft.Json;

namespace AdminApi.Tests.Integration.Models;

public class ListTiersResponse
{
    public List<TierDTO>? Result { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Error? Error { get; set; }
}
