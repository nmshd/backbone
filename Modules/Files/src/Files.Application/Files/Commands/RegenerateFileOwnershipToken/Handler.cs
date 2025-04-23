using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Files.Application.Files.DTOs;
using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Files.Domain.Entities;
using MediatR;
using File = System.IO.File;

namespace Backbone.Modules.Files.Application.Files.Commands.RegenerateFileOwnershipToken;

public class Handler : IRequestHandler<RegenerateFileOwnershipTokenCommand, FileOwnershipTokenDTO>
{
    private readonly IFilesRepository _filesRepository;

    public Handler(IFilesRepository filesRepository)
    {
        _filesRepository = filesRepository;
    }

    public async Task<FileOwnershipTokenDTO> Handle(RegenerateFileOwnershipTokenCommand request, CancellationToken cancellationToken)
    {
        var file = await _filesRepository.Find(FileId.Parse(request.Id), cancellationToken, fillContent: false) ?? throw new NotFoundException(nameof(File));
        file!.RegenerateOwnershipToken();
        _filesRepository.Update(file, cancellationToken);

        return new FileOwnershipTokenDTO(file);
    }
}
