using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Files.Domain.Entities;
using MediatR;
using File = System.IO.File;

namespace Backbone.Modules.Files.Application.Files.Commands.RegenerateFileOwnershipToken;

public class Handler : IRequestHandler<RegenerateFileOwnershipTokenCommand, RegenerateFileOwnershipTokenResponse>
{
    private readonly IFilesRepository _filesRepository;
    private readonly IdentityAddress _activeIdentity;

    public Handler(IFilesRepository filesRepository, IUserContext userContext)
    {
        _filesRepository = filesRepository;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task<RegenerateFileOwnershipTokenResponse> Handle(RegenerateFileOwnershipTokenCommand request, CancellationToken cancellationToken)
    {
        var file = await _filesRepository.Find(FileId.Parse(request.FileId), cancellationToken, fillContent: false) ?? throw new NotFoundException(nameof(File));

        file.RegenerateOwnershipToken(_activeIdentity);
        await _filesRepository.Update(file, cancellationToken);

        return new RegenerateFileOwnershipTokenResponse(file);
    }
}
