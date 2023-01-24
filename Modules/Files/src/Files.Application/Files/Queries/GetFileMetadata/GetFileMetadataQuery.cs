using Files.Application.Files.DTOs;
using Files.Domain.Entities;
using MediatR;

namespace Files.Application.Files.Queries.GetFileMetadata;

public class GetFileMetadataQuery : IRequest<FileMetadataDTO>
{
    public FileId Id { get; set; }
}
