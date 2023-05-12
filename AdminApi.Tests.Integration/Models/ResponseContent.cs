using Newtonsoft.Json;

namespace AdminApi.Tests.Integration.Models;

public class ResponseContent<T>
{
    public T? Result { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Error? Error { get; set; }
}
