using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Files.Abstractions;
using Backbone.Modules.Files.Domain.Entities;
using Backbone.Modules.Files.Module.Features.Files.Shared;
using MediatR;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Module.Features.Files.GetFileMetadata;

public class Handler : IRequestHandler<Query, FileMetadataDTO>
{
    private readonly IFilesRepository _filesRepository;

    public Handler(IFilesRepository filesRepository)
    {
        _filesRepository = filesRepository;
    }

    public async Task<FileMetadataDTO> Handle(Query request, CancellationToken cancellationToken)
    {
        var file = await _filesRepository.Get(FileId.Parse(request.Id), cancellationToken, fillContent: false) ?? throw new NotFoundException(nameof(File));
        return new FileMetadataDTO(file);
    }
}
