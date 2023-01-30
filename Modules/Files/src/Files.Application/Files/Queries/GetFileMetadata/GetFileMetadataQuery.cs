using Backbone.Modules.Files.Application.Files.DTOs;
using Backbone.Modules.Files.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Files.Application.Files.Queries.GetFileMetadata;

public class GetFileMetadataQuery : IRequest<FileMetadataDTO>
{
    public FileId Id { get; set; }
}
