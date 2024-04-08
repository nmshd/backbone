using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Files.Application.Files.DTOs;
using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Files.Application.Files.Queries.GetFileMetadata;

public class Handler : IRequestHandler<GetFileMetadataQuery, FileMetadataDTO>
{
    private readonly IFilesRepository _filesRepository;
    private readonly IMapper _mapper;

    public Handler(IFilesRepository filesRepository, IMapper mapper)
    {
        _filesRepository = filesRepository;
        _mapper = mapper;
    }

    public async Task<FileMetadataDTO> Handle(GetFileMetadataQuery request, CancellationToken cancellationToken)
    {
        var file = await _filesRepository.Find(request.Id, cancellationToken, fillContent: false) ?? throw new NotFoundException(nameof(File));
        return _mapper.Map<FileMetadataDTO>(file);
    }
}
