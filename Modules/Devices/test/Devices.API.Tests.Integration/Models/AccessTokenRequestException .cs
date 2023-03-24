﻿using System.Net;

namespace Devices.API.Tests.Integration.Models;
public class AccessTokenRequestException : Exception
{
    public HttpStatusCode StatusCode { get; set; }
    public string? ErrorMessage { get; set; }

    public AccessTokenRequestException(HttpStatusCode statusCode, string errorMessage)
    {
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
    }
}
