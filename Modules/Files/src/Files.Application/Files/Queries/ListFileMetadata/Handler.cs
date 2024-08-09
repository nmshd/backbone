using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Files.Application.Files.DTOs;
using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Files.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Files.Application.Files.Queries.ListFileMetadata;

public class Handler : IRequestHandler<ListFileMetadataQuery, ListFileMetadataResponse>
{
    private readonly IFilesRepository _filesRepository;
    private readonly IUserContext _userContext;

    public Handler(IFilesRepository filesRepository, IUserContext userContext)
    {
        _filesRepository = filesRepository;
        _userContext = userContext;
    }

    public async Task<ListFileMetadataResponse> Handle(ListFileMetadataQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await _filesRepository.FindFilesByCreator(request.Ids.Select(FileId.Parse), _userContext.GetAddress(), request.PaginationFilter, cancellationToken);
        return new ListFileMetadataResponse(dbPaginationResult.ItemsOnPage.Select(f => new FileMetadataDTO(f)), request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
