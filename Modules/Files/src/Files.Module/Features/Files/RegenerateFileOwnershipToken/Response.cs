using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Module.Features.Files.RegenerateFileOwnershipToken;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("RegenerateFileOwnershipTokenResponse")]
public class Response
{
    public Response(File file)
    {
        NewOwnershipToken = file.OwnershipToken.Value;
    }

    public string NewOwnershipToken { get; set; }
}
