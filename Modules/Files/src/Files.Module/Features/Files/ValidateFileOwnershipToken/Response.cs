namespace Backbone.Modules.Files.Module.Features.Files.ValidateFileOwnershipToken;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ValidateFileOwnershipTokenResponse")]
public class Response
{
    public required bool IsValid { get; init; }
}
