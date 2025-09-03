using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Application.Files.Commands.ExecuteHousekeeping;

public class Handler : IRequestHandler<ExecuteHousekeepingCommand>
{
    private readonly IFilesRepository _tokensRepository;
    private readonly ILogger<Handler> _logger;

    public Handler(IFilesRepository tokensRepository, ILogger<Handler> logger)
    {
        _tokensRepository = tokensRepository;
        _logger = logger;
    }

    public async Task Handle(ExecuteHousekeepingCommand request, CancellationToken cancellationToken)
    {
        await DeleteFiles(cancellationToken);
        await DeleteOrphanedBlobs(cancellationToken);
    }

    private async Task DeleteFiles(CancellationToken cancellationToken)
    {
        var numberOfDeletedFiles = await _tokensRepository.Delete(File.CanBeCleanedUp, cancellationToken);

        _logger.LogInformation("Deleted {NumberOfDeletedChallenges} challenges", numberOfDeletedFiles);
    }

    private async Task DeleteOrphanedBlobs(CancellationToken cancellationToken)
    {
        var numberOfDeletedBlobs = await _tokensRepository.DeleteOrphanedBlobs(cancellationToken);

        _logger.LogInformation("Deleted {NumberOfDeletedChallenges} orphaned blobs", numberOfDeletedBlobs);
    }
}
