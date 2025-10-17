using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using MediatR;
using static Backbone.Modules.Files.Domain.Entities.File;

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
        await _filesRepository.DeleteFilesOfIdentity(IsOwnedBy(request.IdentityAddress), cancellationToken);
    }
}
