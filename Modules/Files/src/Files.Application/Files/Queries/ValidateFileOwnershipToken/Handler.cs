using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Files.Domain.Entities;
using MediatR;
using File = System.IO.File;

namespace Backbone.Modules.Files.Application.Files.Queries.ValidateFileOwnershipToken;

public class Handler : IRequestHandler<ValidateFileOwnershipTokenQuery, bool>
{
    private readonly IFilesRepository _filesRepository;
    private readonly IdentityAddress _activeIdentity;

    public Handler(IFilesRepository filesRepository, IUserContext userContext)
    {
        _filesRepository = filesRepository;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task<bool> Handle(ValidateFileOwnershipTokenQuery request, CancellationToken cancellationToken)
    {
        var file = await _filesRepository.Find(FileId.Parse(request.FileId), cancellationToken) ?? throw new NotFoundException(nameof(File));

        if (_activeIdentity != file.Owner) throw new ActionForbiddenException();

        if (file.FileOwnershipIsLocked)
            return false;

        return request.OwnershipToken.Equals(file.OwnershipToken.Value);
    }
}
