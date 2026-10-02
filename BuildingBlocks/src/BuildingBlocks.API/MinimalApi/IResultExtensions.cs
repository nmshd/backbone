using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Backbone.BuildingBlocks.API.MinimalApi;

public static class IResultExtensions
{
    public static IResult WithHttpCaching(this IResult result) => new HttpCachingResult(result);

    private sealed class HttpCachingResult(IResult result) : IResult
    {
        private static readonly string[] HEADERS_TO_KEEP_FOR304 =
        [
            HeaderNames.CacheControl, HeaderNames.ContentLocation, HeaderNames.ETag, HeaderNames.Expires, HeaderNames.Vary
        ];

        public async Task ExecuteAsync(HttpContext context)
        {
            var response = context.Response;
            var originalStream = response.Body;
            using var buffer = new MemoryStream();
            try
            {
                response.Body = buffer;
                await result.ExecuteAsync(context);
                buffer.Position = 0;

                if (response.StatusCode == StatusCodes.Status200OK)
                {
                    var responseHeaders = response.GetTypedHeaders();
                    responseHeaders.ETag ??= new EntityTagHeaderValue($"\"{Convert.ToBase64String(MD5.HashData(buffer))}\"");
                    buffer.Position = 0;
                    var requestHeaders = context.Request.GetTypedHeaders();
                    var cacheValid = requestHeaders.IfNoneMatch.Any()
                        ? requestHeaders.IfNoneMatch.Any(etag => etag.Compare(responseHeaders.ETag, useStrongComparison: false))
                        : requestHeaders.IfModifiedSince is not null && responseHeaders.LastModified is not null &&
                          requestHeaders.IfModifiedSince >= responseHeaders.LastModified;

                    if (cacheValid)
                    {
                        response.StatusCode = StatusCodes.Status304NotModified;
                        foreach (var header in response.Headers.Keys.Where(key => !HEADERS_TO_KEEP_FOR304.Contains(key)).ToArray())
                            response.Headers.Remove(header);
                        return;
                    }
                }

                await buffer.CopyToAsync(originalStream, context.RequestAborted);
            }
            finally
            {
                response.Body = originalStream;
            }
        }
    }
}
