using Backbone.BuildingBlocks.Application.Housekeeping;
using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using MediatR;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Application.Files.Commands.ExecuteHousekeeping;

public class Handler : IRequestHandler<ExecuteHousekeepingCommand>
{
    private readonly IFilesRepository _filesRepository;
    private readonly HousekeepingTelemetry _telemetry;

    public Handler(IFilesRepository filesRepository, HousekeepingTelemetry telemetry)
    {
        _filesRepository = filesRepository;
        _telemetry = telemetry;
    }

    public async Task Handle(ExecuteHousekeepingCommand request, CancellationToken cancellationToken)
    {
        await DeleteFiles(cancellationToken);
        await DeleteOrphanedBlobs(cancellationToken);
    }

    private async Task DeleteFiles(CancellationToken cancellationToken)
    {
        await _telemetry.TrackDeletion("files", ct => _filesRepository.Delete(File.CanBeCleanedUp, ct), cancellationToken);
    }

    private async Task DeleteOrphanedBlobs(CancellationToken cancellationToken)
    {
        await _telemetry.TrackDeletion("orphaned file contents", _filesRepository.DeleteOrphanedBlobs, cancellationToken);
    }
}
