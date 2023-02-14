using AutoMapper;
using Backbone.Modules.Files.Application.Infrastructure.Persistence;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Files.Application.Files.Queries.GetFileContent;

public class Handler : RequestHandlerBase<GetFileContentQuery, GetFileContentResponse>
{
    private readonly IBlobStorage _blobStorage;
    private readonly BlobOptions _blobOptions;

    public Handler(IFilesDbContext dbContext, IUserContext userContext, IMapper mapper, IBlobStorage blobStorage, IOptions<BlobOptions> blobOptions) : base(dbContext, userContext, mapper)
    {
        _blobStorage = blobStorage;
        _blobOptions = blobOptions.Value;
    }

    public override async Task<GetFileContentResponse> Handle(GetFileContentQuery request, CancellationToken cancellationToken)
    {
        await ValidateFileExistsInDatabase(request.Id);

        var content = await _blobStorage.FindAsync(_blobOptions.RootFolder, request.Id);
        return new GetFileContentResponse
        {
            FileContent = content
        };
    }
}
