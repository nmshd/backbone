using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Backbone.BuildingBlocks.API.MinimalApi;

public static class EndpointRouteBuilderExtensions
{
    public static RouteGroupBuilder MapVersionedEndpointGroup(this IEndpointRouteBuilder endpoints, string route, string tag, int apiVersion,
        string authorizationPolicy)
    {
        var version = new ApiVersion(apiVersion);
        var versionSet = endpoints.NewApiVersionSet()
            .HasApiVersion(version)
            .ReportApiVersions()
            .Build();

        return endpoints.MapGroup($"api/v{{v:apiVersion}}/{route}")
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(version)
            .RequireAuthorization(authorizationPolicy)
            .WithTags(tag)
            .WithMetadata(MinimalApiEndpointMetadata.INSTANCE);
    }
}

internal sealed class MinimalApiEndpointMetadata
{
    public static readonly MinimalApiEndpointMetadata INSTANCE = new();

    private MinimalApiEndpointMetadata()
    {
    }
}
