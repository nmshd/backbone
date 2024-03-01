using Newtonsoft.Json;

namespace Backbone.AdminApi.Tests.Integration.Models;

public class ResponseContent<T>
{
    public T? Result { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Error? Error { get; set; }
}

public class ErrorResponseContent
{

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public required Error Error { get; set; }
}
