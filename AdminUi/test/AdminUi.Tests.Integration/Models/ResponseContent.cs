using Newtonsoft.Json;

namespace AdminUi.Tests.Integration.Models;

public class ResponseContent<T>
{
    public T? Result { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Error? Error { get; set; }
}
