using Backbone.Modules.Files.Application.Files.DTOs;
using MediatR;

namespace Backbone.Modules.Files.Application.Files.Queries.GetFileMetadata;

public class GetFileMetadataQuery : IRequest<FileMetadataDTO>
{
    public required string Id { get; init; }
}
