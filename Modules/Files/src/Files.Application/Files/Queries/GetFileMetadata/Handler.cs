using AutoMapper;
using Backbone.Modules.Files.Application.Files.DTOs;
using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Files.Application.Files.Queries.GetFileMetadata;

public class Handler : IRequestHandler<GetFileMetadataQuery, FileMetadataDTO>
{
    private readonly IFilesRepository _filesRepository;
    private readonly IMapper _mapper;

    public Handler(IFilesRepository filesRepository, IMapper mapper) {
        _filesRepository = filesRepository;
        _mapper = mapper;
    }

    public async Task<FileMetadataDTO> Handle(GetFileMetadataQuery request, CancellationToken cancellationToken)
    {
        var file = await _filesRepository.Find(request.Id, fillContent: false);

        return _mapper.Map<FileMetadataDTO>(file);
    }
}
