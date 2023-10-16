﻿using Newtonsoft.Json;

namespace AdminUi.Tests.Integration.Models;

public class RequestConfiguration
{
    public bool Authenticate { get; set; }
    public string? ContentType { get; set; }
    public string? AcceptHeader { get; set; }
    public string? Content { get; set; }
    public bool IsODataRequest { get; set; }

    public void SetContent(object obj)
    {
        Content = JsonConvert.SerializeObject(obj);
    }

    public void SupplementWith(RequestConfiguration other)
    {
        Authenticate = other.Authenticate;
        AcceptHeader ??= other.AcceptHeader;
        ContentType ??= other.ContentType;
        Content ??= other.Content;
        IsODataRequest = other.IsODataRequest;
    }

    public RequestConfiguration Clone()
    {
        var requestConfiguration = new RequestConfiguration();
        requestConfiguration.SupplementWith(this);
        return requestConfiguration;
    }
}
