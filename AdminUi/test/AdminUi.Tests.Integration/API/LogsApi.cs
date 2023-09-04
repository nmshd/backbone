﻿using AdminUi.Tests.Integration.Configuration;
using AdminUi.Tests.Integration.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Options;

namespace AdminUi.Tests.Integration.API;
public class LogsApi : BaseApi
{
    public LogsApi(IOptions<HttpClientOptions> httpConfiguration, WebApplicationFactory<Program> factory) : base(httpConfiguration, factory) { }

    public async Task<HttpResponse> CreateLog(RequestConfiguration requestConfiguration)
    {
        return await Post($"/Logs", requestConfiguration);
    }
}
