using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain;

namespace Backbone.Modules.Quotas.Application.Metrics;
public class UsedFileStorageSpaceMetricCalculator : IMetricCalculator
{
    private readonly IFilesRepository _filesRepository;

    public UsedFileStorageSpaceMetricCalculator(IFilesRepository filesRepository)
    {
        _filesRepository = filesRepository;
    }

    public async Task<uint> CalculateUsage(DateTime from, DateTime to, string uploader, CancellationToken cancellationToken)
    {
        var usedSpace = await _filesRepository.AggregateUsedSpace(uploader, from, to, cancellationToken);
        return (uint)(usedSpace / 1024 / 1024);
    }
}
