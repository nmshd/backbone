using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Files.Application.Identities.Commands.DeleteFilesOfIdentity;

public class Handler(IFilesRepository filesRepository) : IRequestHandler<DeleteFilesOfIdentityCommand>
{
    public async Task Handle(DeleteFilesOfIdentityCommand request, CancellationToken cancellationToken)
    {
        await filesRepository.DeleteFilesOfIdentity(request.IdentityAddress, cancellationToken);
    }
}
