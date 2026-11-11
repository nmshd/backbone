using System.Text.Json;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.API.MinimalApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Backbone.BuildingBlocks.API.Tests.OpenApi;

public class SwaggerTests : AbstractTestsBase
{
    [Fact]
    public async Task Documents_use_OpenApi_3_2_and_describe_base64_encoding()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { ContentRootPath = AppContext.BaseDirectory });
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Services.AddMinimalApiErrorHandling();
        builder.Services.AddCustomSwaggerUi(new SwaggerUiConfiguration { Enabled = true }, "Test API");

        await using var app = builder.Build();
        app.UseCustomSwaggerUi();
        app.MapGet("/binary", (Base64QueryValue content) => new BinaryPayload(content.Value)).WithGroupName("v1");
        app.MapGet("/optional", (Base64QueryValue? content) => new BinaryPayload(content?.Value)).WithGroupName("v1");
        await app.StartAsync(TestContext.Current.CancellationToken);

        using var client = new HttpClient { BaseAddress = new Uri(app.Urls.Single()) };
        using var response = await client.GetAsync("/docs/v1/openapi.json", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        var document = json.RootElement;

        document.GetProperty("openapi").GetString().ShouldBe("3.2.0");
        document.GetProperty("info").GetProperty("title").GetString().ShouldBe("Test API");
        var operation = document.GetProperty("paths").GetProperty("/binary").GetProperty("get");
        operation.GetProperty("parameters")[0].GetProperty("schema").GetProperty("contentEncoding").GetString().ShouldBe("base64");
        var optionalParameter = document.GetProperty("paths").GetProperty("/optional").GetProperty("get").GetProperty("parameters")[0];
        (optionalParameter.TryGetProperty("required", out var required) && required.GetBoolean()).ShouldBeFalse();
        optionalParameter.GetProperty("schema").GetProperty("contentEncoding").GetString().ShouldBe("base64");
        document.GetProperty("components").GetProperty("schemas").GetProperty(nameof(BinaryPayload))
            .GetProperty("properties").GetProperty("content").GetProperty("contentEncoding").GetString().ShouldBe("base64");
        document.GetProperty("components").GetProperty("securitySchemes").GetProperty("oauth2")
            .GetProperty("flows").GetProperty("password").GetProperty("tokenUrl").GetString().ShouldBe("/connect/token");

        using var ui = await client.GetAsync("/docs/index.html", TestContext.Current.CancellationToken);
        ui.EnsureSuccessStatusCode();
    }

    public record BinaryPayload(byte[]? Content);
}
