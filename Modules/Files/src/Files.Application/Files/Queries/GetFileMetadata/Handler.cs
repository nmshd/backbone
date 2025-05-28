using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Files.Application.Files.DTOs;
using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Files.Domain.Entities;
using MediatR;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Application.Files.Queries.GetFileMetadata;

public class Handler : IRequestHandler<GetFileMetadataQuery, FileMetadataDTO>
{
    private readonly IFilesRepository _filesRepository;

    public Handler(IFilesRepository filesRepository)
    {
        _filesRepository = filesRepository;
    }

    public async Task<FileMetadataDTO> Handle(GetFileMetadataQuery request, CancellationToken cancellationToken)
    {
        var file = await _filesRepository.Get(FileId.Parse(request.Id), cancellationToken, fillContent: false) ?? throw new NotFoundException(nameof(File));
        return new FileMetadataDTO(file);
    }
}
