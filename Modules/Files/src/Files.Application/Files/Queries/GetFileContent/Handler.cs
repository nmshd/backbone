﻿using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using MediatR;

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
        var file = await _filesRepository.Find(request.Id, cancellationToken) ?? throw new NotFoundException(nameof(File));
        return new GetFileContentResponse()
        {
            FileContent = file.Content
        };
    }
}
