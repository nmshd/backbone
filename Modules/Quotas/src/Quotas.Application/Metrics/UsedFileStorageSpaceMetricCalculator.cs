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

    public async Task<uint> CalculateUsage(DateTime from, DateTime to, string owner, CancellationToken cancellationToken)
    {
        var usedSpace = await _filesRepository.AggregateUsedSpace(owner, from, to, cancellationToken);
        return Convert.ToUInt32(usedSpace / 1024 / 1024);
    }
}
