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
    private readonly IdentityAddress _activeUserAdress;

    public Handler(IFilesRepository filesRepository, IUserContext userContext)
    {
        _filesRepository = filesRepository;
        _activeUserAdress = userContext.GetAddress();
    }

    public async Task<ClaimFileOwnershipResponse> Handle(ClaimFileOwnershipCommand request, CancellationToken cancellationToken)
    {
        var file = await _filesRepository.Find(FileId.Parse(request.FileId), cancellationToken, true, false) ?? throw new NotFoundException(nameof(File));
        try
        {
            var newOwnershipToken = file.ClaimOwnership(request.OwnershipToken!, _activeUserAdress);
            await _filesRepository.Update(file, CancellationToken.None);
            return new ClaimFileOwnershipResponse(newOwnershipToken);
        }
        catch (DomainException)
        {
            await _filesRepository.Update(file, CancellationToken.None);
            throw new DomainActionForbiddenException();
        }
    }
}
