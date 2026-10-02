using Backbone.Modules.Files.Module.Features.Files.Shared;
using MediatR;

namespace Backbone.Modules.Files.Module.Features.Files.GetFileMetadata;

public class GetFileMetadataQuery : IRequest<FileMetadataDTO>
{
    public required string Id { get; init; }
}
