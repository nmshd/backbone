﻿using Newtonsoft.Json;

namespace ConsumerApi.Tests.Integration.Models;

public class ResponseContent<T>
{
    public T? Result { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Error? Error { get; set; }
}
