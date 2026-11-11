using System.Text.Json;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.SseServer.Controllers;
using Backbone.SseServer.OpenApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Backbone.SseServerTests.Controllers;

public class SseDocumentationTests : AbstractTestsBase
{
    [Theory]
    [InlineData("v1")]
    [InlineData("v2")]
    public async Task Subscribe_describes_each_server_sent_event(string version)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { ContentRootPath = AppContext.BaseDirectory });
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Services.AddControllers().AddApplicationPart(typeof(SseController).Assembly);
        builder.Services.AddApiVersioning().AddMvc();
        builder.Services.AddCustomSwaggerUi(new SwaggerUiConfiguration { Enabled = true }, "SSE Server");
        builder.Services.Configure<SwaggerGenOptions>(options => options.OperationFilter<SseOperationFilter>());

        await using var app = builder.Build();
        app.UseCustomSwaggerUi();
        app.MapControllers();
        await app.StartAsync(TestContext.Current.CancellationToken);

        using var client = new HttpClient { BaseAddress = new Uri(app.Urls.Single()) };
        using var response = await client.GetAsync($"/docs/{version}/openapi.json", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        var document = json.RootElement;

        document.GetProperty("openapi").GetString().ShouldBe("3.2.0");
        var content = document.GetProperty("paths").GetProperty($"/api/{version}/sse").GetProperty("get")
            .GetProperty("responses").GetProperty("200").GetProperty("content");
        content.EnumerateObject().Select(property => property.Name).ShouldBe(["text/event-stream"]);
        var mediaType = content.GetProperty("text/event-stream");
        mediaType.TryGetProperty("schema", out _).ShouldBeFalse();
        var itemSchema = mediaType.GetProperty("itemSchema");
        itemSchema.GetProperty("type").GetString().ShouldBe("object");
        itemSchema.GetProperty("required").EnumerateArray().Select(value => value.GetString()).ShouldBe(["event", "data"], ignoreOrder: true);
        itemSchema.GetProperty("properties").GetProperty("event").GetProperty("examples")[0].GetString().ShouldBe("ConnectionOpened");
        itemSchema.GetProperty("properties").GetProperty("data").GetProperty("const").GetString().ShouldBe("_");

        await using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);
        await writer.SendServerSentEvent("ConnectionOpened");
        var actualEvent = System.Text.Encoding.UTF8.GetString(stream.ToArray());
        mediaType.GetProperty("examples").GetProperty("connectionOpened").GetProperty("serializedValue").GetString()
            .ShouldBe(actualEvent.ReplaceLineEndings("\n"));

        // The filter must leave ordinary JSON/empty responses alone.
        if (version == "v1")
        {
            document.GetProperty("paths").GetProperty("/{address}/events").GetProperty("post")
                .GetProperty("responses").GetProperty("200").TryGetProperty("content", out _).ShouldBeFalse();
        }
    }
}
