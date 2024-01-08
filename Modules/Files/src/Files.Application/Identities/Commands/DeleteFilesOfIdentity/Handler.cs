using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Files.Application.Identities.Commands.DeleteFilesOfIdentity;

public class Handler : IRequestHandler<DeleteFilesOfIdentityCommand>
{
    private readonly IFilesRepository _filesRepository;

    public Handler(IFilesRepository filesRepository)
    {
        _filesRepository = filesRepository;
    }

    public async Task Handle(DeleteFilesOfIdentityCommand request, CancellationToken cancellationToken)
    {
        await _filesRepository.DeleteFilesOfIdentity(request.IdentityAddress, cancellationToken);
    }
}
