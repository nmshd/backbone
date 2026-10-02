using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.Modules.Files.Module.Features.Files.CreateFile;
using Backbone.Modules.Files.Module.Features.Files.GetFileContent;
using Backbone.Modules.Files.Module.Features.Files.GetFileMetadata;
using Backbone.Modules.Files.Module.Features.Files.RegenerateFileOwnershipToken;
using Backbone.Modules.Files.Module.Features.Files.ClaimFileOwnership;
using Backbone.Modules.Files.Module.Features.Files.ValidateFileOwnershipToken;
using Backbone.Modules.Files.Module.Features.Files.DeleteFile;
using Backbone.Modules.Files.Module.Features.Files.ListFileMetadata;
using Microsoft.AspNetCore.Routing;
using OpenIddict.Validation.AspNetCore;

namespace Backbone.Modules.Files.Module;

public static class FilesEndpointExtensions
{
    public static IEndpointRouteBuilder MapFilesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedEndpointGroup("Files", "Files", 2, OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        group.MapCreateFileEndpoint();
        group.MapGetFileContentEndpoint();
        group.MapGetFileMetadataEndpoint();
        group.MapRegenerateFileOwnershipTokenEndpoint();
        group.MapClaimFileOwnershipEndpoint();
        group.MapValidateFileOwnershipTokenEndpoint();
        group.MapDeleteFileEndpoint();
        group.MapListFileMetadataEndpoint();
        return endpoints;
    }
}
