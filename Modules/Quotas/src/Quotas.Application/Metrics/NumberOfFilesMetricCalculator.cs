using Backbone.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Quotas.Domain;

namespace Backbone.Quotas.Application.Metrics;
public class NumberOfFilesMetricCalculator : IMetricCalculator
{
    private readonly IFilesRepository _filesRepository;

    public NumberOfFilesMetricCalculator(IFilesRepository filesRepository)
    {
        _filesRepository = filesRepository;
    }

    public async Task<uint> CalculateUsage(DateTime from, DateTime to, string uploader, CancellationToken cancellationToken)
    {
        var numberOfFiles = await _filesRepository.Count(uploader, from, to, cancellationToken);
        return numberOfFiles;
    }
}
