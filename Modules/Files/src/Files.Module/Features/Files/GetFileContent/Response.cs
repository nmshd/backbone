namespace Backbone.Modules.Files.Module.Features.Files.GetFileContent;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("GetFileContentResponse")]
public class Response
{
    public required byte[] FileContent { get; set; }
}
