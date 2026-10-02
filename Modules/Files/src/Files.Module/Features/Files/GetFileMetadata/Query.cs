using Backbone.Modules.Files.Module.Features.Files.Shared;
using MediatR;

namespace Backbone.Modules.Files.Module.Features.Files.GetFileMetadata;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("GetFileMetadataQuery")]
public class Query : IRequest<FileMetadataDTO>
{
    public required string Id { get; init; }
}
