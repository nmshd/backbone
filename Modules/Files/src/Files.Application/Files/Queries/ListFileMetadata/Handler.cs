using AutoMapper;
using Backbone.Modules.Files.Application.Files.DTOs;
using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;

namespace Backbone.Modules.Files.Application.Files.Queries.ListFileMetadata;

public class Handler : IRequestHandler<ListFileMetadataQuery, ListFileMetadataResponse>
{
    private readonly IFilesRepository _filesRepository;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public Handler(IFilesRepository filesRepository, IUserContext userContext, IMapper mapper)
    {
        _filesRepository = filesRepository;
        _userContext = userContext;
        _mapper = mapper;
    }


    public async Task<ListFileMetadataResponse> Handle(ListFileMetadataQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await _filesRepository.FindFilesByCreator(request.Ids, _userContext.GetAddress(), request.PaginationFilter);
        
        var items = _mapper.Map<FileMetadataDTO[]>(dbPaginationResult.ItemsOnPage);

        var response = new ListFileMetadataResponse(items, request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);

        return response;
    }
}
