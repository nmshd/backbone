using AutoMapper;
using AutoMapper.QueryableExtensions;
using Backbone.Modules.Files.Application.Extensions;
using Backbone.Modules.Files.Application.Files.DTOs;
using Backbone.Modules.Files.Application.Infrastructure.Persistence;
using Backbone.Modules.Files.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;

namespace Backbone.Modules.Files.Application.Files.Queries.GetFileMetadata;

public class Handler : RequestHandlerBase<GetFileMetadataQuery, FileMetadataDTO>
{
    public Handler(IFilesDbContext dbContext, IUserContext userContext, IMapper mapper) : base(dbContext, userContext, mapper) { }

    public override async Task<FileMetadataDTO> Handle(GetFileMetadataQuery request, CancellationToken cancellationToken)
    {
        var metadata = await _dbContext
            .SetReadOnly<FileMetadata>()
            .NotExpired()
            .NotDeleted()
            .ProjectTo<FileMetadataDTO>(_mapper.ConfigurationProvider)
            .FirstWithId(request.Id, cancellationToken);

        return metadata;
    }
}
