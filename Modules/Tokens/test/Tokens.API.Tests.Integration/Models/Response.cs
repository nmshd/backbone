using Newtonsoft.Json;

namespace Tokens.API.Tests.Integration.Models;
public class Response<T>
{
    public T? Result { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Error? Error { get; set; }
}
