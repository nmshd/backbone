using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Files.Abstractions;
using Backbone.Modules.Files.Domain.Entities;
using MediatR;
using File = System.IO.File;

namespace Backbone.Modules.Files.Module.Features.Files.RegenerateFileOwnershipToken;

public class Handler : IRequestHandler<Command, Response>
{
    private readonly IFilesRepository _filesRepository;
    private readonly IdentityAddress _activeIdentity;

    public Handler(IFilesRepository filesRepository, IUserContext userContext)
    {
        _filesRepository = filesRepository;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var file = await _filesRepository.Get(FileId.Parse(request.FileId), cancellationToken, fillContent: false) ?? throw new NotFoundException(nameof(File));

        file.RegenerateOwnershipToken(_activeIdentity);
        await _filesRepository.Update(file, cancellationToken);

        return new Response(file);
    }
}
