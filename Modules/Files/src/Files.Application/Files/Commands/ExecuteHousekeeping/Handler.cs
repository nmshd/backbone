using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Application.Files.Commands.ExecuteHousekeeping;

public class Handler : IRequestHandler<ExecuteHousekeepingCommand>
{
    private readonly IFilesRepository _filesRepository;
    private readonly ILogger<Handler> _logger;

    public Handler(IFilesRepository filesRepository, ILogger<Handler> logger)
    {
        _filesRepository = filesRepository;
        _logger = logger;
    }

    public async Task Handle(ExecuteHousekeepingCommand request, CancellationToken cancellationToken)
    {
        await DeleteFiles(cancellationToken);
        await DeleteOrphanedBlobs(cancellationToken);
    }

    private async Task DeleteFiles(CancellationToken cancellationToken)
    {
        var numberOfDeletedFiles = await _filesRepository.Delete(File.CanBeCleanedUp, cancellationToken);

        _logger.LogInformation("Deleted {numberOfDeletedItems} files", numberOfDeletedFiles);
    }

    private async Task DeleteOrphanedBlobs(CancellationToken cancellationToken)
    {
        var numberOfDeletedBlobs = await _filesRepository.DeleteOrphanedBlobs(cancellationToken);

        _logger.LogInformation("Deleted {numberOfDeletedItems} orphaned blobs", numberOfDeletedBlobs);
    }
}
