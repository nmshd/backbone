using AutoMapper;
using Backbone.Modules.Files.Application.Infrastructure.Persistence;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;

namespace Backbone.Modules.Files.Application.Files.Queries.GetFileContent;

public class Handler : RequestHandlerBase<GetFileContentQuery, GetFileContentResponse>
{
    private readonly IBlobStorage _blobStorage;

    public Handler(IFilesDbContext dbContext, IUserContext userContext, IMapper mapper, IBlobStorage blobStorage) : base(dbContext, userContext, mapper)
    {
        _blobStorage = blobStorage;
    }

    public override async Task<GetFileContentResponse> Handle(GetFileContentQuery request, CancellationToken cancellationToken)
    {
        await ValidateFileExistsInDatabase(request.Id);

        var content = await _blobStorage.FindAsync(request.Id);
        return new GetFileContentResponse
        {
            FileContent = content
        };
    }
}
