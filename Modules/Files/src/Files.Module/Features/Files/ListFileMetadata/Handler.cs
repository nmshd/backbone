using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Files.Abstractions;
using Backbone.Modules.Files.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Files.Module.Features.Files.ListFileMetadata;

public class Handler : IRequestHandler<Query, Response>
{
    private readonly IFilesRepository _filesRepository;
    private readonly IUserContext _userContext;

    public Handler(IFilesRepository filesRepository, IUserContext userContext)
    {
        _filesRepository = filesRepository;
        _userContext = userContext;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await _filesRepository.ListFilesByCreator(request.Ids.Select(FileId.Parse), _userContext.GetAddress(), request.PaginationFilter, cancellationToken);
        return new Response(dbPaginationResult, request.PaginationFilter);
    }
}
