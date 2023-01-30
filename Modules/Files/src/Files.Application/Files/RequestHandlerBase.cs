using AutoMapper;
using Backbone.Modules.Files.Application.Extensions;
using Backbone.Modules.Files.Application.Infrastructure.Persistence;
using Backbone.Modules.Files.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Files.Application.Files;

public abstract class RequestHandlerBase<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    protected readonly IdentityAddress _activeIdentity;
    protected readonly IFilesDbContext _dbContext;
    protected readonly IMapper _mapper;

    protected RequestHandlerBase(IFilesDbContext dbContext, IUserContext userContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _activeIdentity = userContext.GetAddress();
    }

    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);

    protected async Task ValidateFileExistsInDatabase(FileId fileId)
    {
        var fileExists = await _dbContext
            .SetReadOnly<FileMetadata>()
            .WithId(fileId)
            .NotExpired()
            .NotDeleted()
            .AnyAsync();

        if (!fileExists)
            throw new NotFoundException("File");
    }
}
