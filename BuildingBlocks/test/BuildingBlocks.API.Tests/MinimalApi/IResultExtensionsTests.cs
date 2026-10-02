using System.Text;
using Backbone.BuildingBlocks.API.MinimalApi;
using Microsoft.AspNetCore.Http;

namespace Backbone.BuildingBlocks.API.Tests.MinimalApi;

public class IResultExtensionsTests : AbstractTestsBase
{
    [Fact]
    public async Task Exceptions_restore_original_response_stream()
    {
        var context = CreateContext();
        var original = context.Response.Body;
        var result = new TestResult(_ => throw new InvalidOperationException("Serialization failed"));
        await Should.ThrowAsync<InvalidOperationException>(() => result.WithHttpCaching().ExecuteAsync(context));
        context.Response.Body.ShouldBeSameAs(original);
    }

    [Fact]
    public async Task Non_success_responses_are_copied_without_etag()
    {
        var context = CreateContext();
        var original = context.Response.Body;
        await new TestResult(async ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
            await ctx.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("error"), ctx.RequestAborted);
        }).WithHttpCaching().ExecuteAsync(context);
        context.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        context.Response.Headers.ETag.Count.ShouldBe(0);
        context.Response.Body.ShouldBeSameAs(original);
        Encoding.UTF8.GetString(((MemoryStream)original).ToArray()).ShouldBe("error");
    }

    [Theory]
    [InlineData(false, 304)]
    [InlineData(true, 200)]
    public async Task If_none_match_takes_precedence_over_if_modified_since(bool withNonmatchingEtag, int expectedStatus)
    {
        var context = CreateContext();
        var modified = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        context.Request.GetTypedHeaders().IfModifiedSince = modified.AddDays(1);
        if (withNonmatchingEtag)
            context.Request.Headers.IfNoneMatch = "\"old\"";
        await new TestResult(async ctx =>
        {
            ctx.Response.GetTypedHeaders().LastModified = modified;
            await ctx.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("result"), ctx.RequestAborted);
        }).WithHttpCaching().ExecuteAsync(context);
        context.Response.StatusCode.ShouldBe(expectedStatus);
        ((MemoryStream)context.Response.Body).Length.ShouldBe(expectedStatus == 304 ? 0 : 6);
    }

    [Fact]
    public async Task Matching_existing_etag_preserves_only_cache_headers()
    {
        var context = CreateContext();
        context.Request.Headers.IfNoneMatch = "\"existing\"";
        await new TestResult(async ctx =>
        {
            ctx.Response.Headers.ETag = "\"existing\"";
            ctx.Response.Headers.CacheControl = "public, max-age=60";
            ctx.Response.Headers.Vary = "Accept-Encoding";
            ctx.Response.ContentType = "application/json";
            ctx.Response.ContentLength = 6;
            await ctx.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("result"), ctx.RequestAborted);
        }).WithHttpCaching().ExecuteAsync(context);
        context.Response.StatusCode.ShouldBe(StatusCodes.Status304NotModified);
        context.Response.Headers.ETag.ToString().ShouldBe("\"existing\"");
        context.Response.Headers.CacheControl.ToString().ShouldBe("public, max-age=60");
        context.Response.Headers.Vary.ToString().ShouldBe("Accept-Encoding");
        context.Response.ContentType.ShouldBeNull();
        context.Response.ContentLength.ShouldBeNull();
        ((MemoryStream)context.Response.Body).Length.ShouldBe(0);
    }

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.RequestAborted = TestContext.Current.CancellationToken;
        return context;
    }

    private sealed class TestResult(Func<HttpContext, Task> execute) : IResult
    {
        public Task ExecuteAsync(HttpContext context) => execute(context);
    }
}
