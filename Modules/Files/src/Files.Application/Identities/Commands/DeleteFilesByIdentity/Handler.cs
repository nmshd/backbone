using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Files.Application.Identities.Commands.DeleteFilesByIdentity;

public class Handler(IFilesRepository filesRepository) : IRequestHandler<DeleteFilesByIdentityCommand>
{
    public async Task Handle(DeleteFilesByIdentityCommand request, CancellationToken cancellationToken)
    {
        await filesRepository.DeleteFilesByIdentity(request.IdentityAddress, cancellationToken);
    }
}
