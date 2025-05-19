using System.Diagnostics;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Files.Domain.Entities;
using MediatR;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Application.Files.Commands.ClaimFileOwnership;

public class Handler : IRequestHandler<ClaimFileOwnershipCommand, ClaimFileOwnershipResponse>
{
    private readonly IFilesRepository _filesRepository;
    private readonly IdentityAddress _activeIdentity;

    public Handler(IFilesRepository filesRepository, IUserContext userContext)
    {
        _filesRepository = filesRepository;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task<ClaimFileOwnershipResponse> Handle(ClaimFileOwnershipCommand request, CancellationToken cancellationToken)
    {
        var file = await _filesRepository.Find(FileId.Parse(request.FileId), cancellationToken, true, false) ?? throw new NotFoundException(nameof(File));

        var result = file.ClaimOwnership(FileOwnershipToken.Parse(request.OwnershipToken), _activeIdentity);

        switch (result)
        {
            case File.ClaimFileOwnershipResult.Ok:
                await _filesRepository.Update(file, cancellationToken);
                break;
            case File.ClaimFileOwnershipResult.WrongToken:
                await _filesRepository.Update(file, cancellationToken);
                throw new DomainActionForbiddenException();
            case File.ClaimFileOwnershipResult.Locked:
                throw new DomainActionForbiddenException();
            default:
                throw new UnreachableException();
        }

        return new ClaimFileOwnershipResponse(file);
    }
}
