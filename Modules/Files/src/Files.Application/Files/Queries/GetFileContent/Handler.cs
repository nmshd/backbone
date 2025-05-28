using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Files.Domain.Entities;
using MediatR;
using File = System.IO.File;

namespace Backbone.Modules.Files.Application.Files.Queries.GetFileContent;

public class Handler : IRequestHandler<GetFileContentQuery, GetFileContentResponse>
{
    private readonly IFilesRepository _filesRepository;

    public Handler(IFilesRepository filesRepository)
    {
        _filesRepository = filesRepository;
    }

    public async Task<GetFileContentResponse> Handle(GetFileContentQuery request, CancellationToken cancellationToken)
    {
        var file = await _filesRepository.Get(FileId.Parse(request.Id), cancellationToken) ?? throw new NotFoundException(nameof(File));
        return new GetFileContentResponse
        {
            FileContent = file.Content
        };
    }
}
