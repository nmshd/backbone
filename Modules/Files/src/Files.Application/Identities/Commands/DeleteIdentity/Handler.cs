using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Files.Application.Identities.Commands.DeleteIdentity;

public class Handler : IRequestHandler<DeleteIdentityCommand>
{
    private readonly IFilesRepository _filesRepository;

    public Handler(IFilesRepository filesRepository)
    {
        _filesRepository = filesRepository;
    }

    public async Task Handle(DeleteIdentityCommand request, CancellationToken cancellationToken)
    {
        await _filesRepository.DeleteFilesByCreator(request.IdentityAddress, cancellationToken);
    }
}
