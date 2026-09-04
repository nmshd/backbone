using Backbone.BuildingBlocks.Application.Housekeeping;
using Backbone.Modules.Files.Application.Infrastructure.Persistence.Repository;
using MediatR;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Application.Files.Commands.ExecuteHousekeeping;

public class Handler : IRequestHandler<ExecuteHousekeepingCommand>
{
    private readonly IFilesRepository _filesRepository;

    public Handler(IFilesRepository filesRepository)
    {
        _filesRepository = filesRepository;
    }

    public async Task Handle(ExecuteHousekeepingCommand request, CancellationToken cancellationToken)
    {
        await DeleteFiles(cancellationToken);
        await DeleteOrphanedBlobs(cancellationToken);
    }

    private async Task DeleteFiles(CancellationToken cancellationToken)
    {
        await HousekeepingTelemetry.TrackItemDeletion("files", ct => _filesRepository.Delete(File.CanBeCleanedUp, ct), cancellationToken);
    }

    private async Task DeleteOrphanedBlobs(CancellationToken cancellationToken)
    {
        await HousekeepingTelemetry.TrackItemDeletion("orphaned file contents", _filesRepository.DeleteOrphanedBlobs, cancellationToken);
    }
}
