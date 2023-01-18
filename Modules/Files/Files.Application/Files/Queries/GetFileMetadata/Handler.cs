using AutoMapper;
using AutoMapper.QueryableExtensions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Files.Application.Extensions;
using Files.Application.Files.DTOs;
using Files.Domain.Entities;

namespace Files.Application.Files.Queries.GetFileMetadata;

public class Handler : RequestHandlerBase<GetFileMetadataQuery, FileMetadataDTO>
{
    public Handler(IDbContext dbContext, IUserContext userContext, IMapper mapper) : base(dbContext, userContext, mapper) { }

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
