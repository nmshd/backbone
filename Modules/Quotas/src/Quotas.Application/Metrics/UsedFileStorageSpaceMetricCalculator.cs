﻿using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
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
        var numberOfFiles = await _filesRepository.UsedSpace(uploader, from, to, cancellationToken);
        return numberOfFiles;
    }
}
