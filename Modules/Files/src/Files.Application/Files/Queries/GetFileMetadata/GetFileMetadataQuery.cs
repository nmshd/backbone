using Backbone.Files.Application.Files.DTOs;
using Backbone.Files.Domain.Entities;
using MediatR;

namespace Backbone.Files.Application.Files.Queries.GetFileMetadata;

public class GetFileMetadataQuery : IRequest<FileMetadataDTO>
{
    public FileId Id { get; set; }
}
