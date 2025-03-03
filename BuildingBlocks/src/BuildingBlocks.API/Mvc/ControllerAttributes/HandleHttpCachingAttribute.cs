using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;

namespace Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;

public class HandleHttpCachingAttribute : ResultFilterAttribute
{
    private static readonly string[] HEADERS_TO_KEEP_FOR304 =
    [
        HeaderNames.CacheControl,
        HeaderNames.ContentLocation,
        HeaderNames.ETag,
        HeaderNames.Expires,
        HeaderNames.Vary
    ];

    public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var request = context.HttpContext.Request;
        var response = context.HttpContext.Response;

        var originalStream = response.Body;
        using MemoryStream memoryStream = new();

        response.Body = memoryStream;
        await next();
        memoryStream.Position = 0;

        if (response.StatusCode == StatusCodes.Status200OK) //TODO: Timo (Is it safe to assume that cached responses always return 200?)
        {
            var requestHeaders = request.GetTypedHeaders();
            var responseHeaders = response.GetTypedHeaders();

            responseHeaders.ETag ??= GenerateETag(memoryStream);

            if (IsClientCacheValid(requestHeaders, responseHeaders))
            {
                response.StatusCode = StatusCodes.Status304NotModified;

                foreach (var header in response.Headers)
                    if (!HEADERS_TO_KEEP_FOR304.Contains(header.Key))
                        response.Headers.Remove(header.Key);

                return;
            }
        }

        await memoryStream.CopyToAsync(originalStream);
    }

    private static EntityTagHeaderValue GenerateETag(Stream stream)
    {
        var hashBytes = MD5.HashData(stream);
        stream.Position = 0;
        var hashString = Convert.ToBase64String(hashBytes);
        return new EntityTagHeaderValue($"\"{hashString}\"");
    }

    private static bool IsClientCacheValid(RequestHeaders reqHeaders, ResponseHeaders resHeaders)
    {
        if (reqHeaders.IfNoneMatch.Any() && resHeaders.ETag is not null)
            return reqHeaders.IfNoneMatch.Any(etag =>
                etag.Compare(resHeaders.ETag, useStrongComparison: false)
            );

        if (reqHeaders.IfModifiedSince is not null && resHeaders.LastModified is not null)
            return reqHeaders.IfModifiedSince >= resHeaders.LastModified;

        return false;
    }
}
