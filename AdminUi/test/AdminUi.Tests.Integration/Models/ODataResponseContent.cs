using Newtonsoft.Json;

namespace AdminUi.Tests.Integration.Models;

public class ODataResponseContent<T>
{
    public T? Value { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Error? Error { get; set; }
}
