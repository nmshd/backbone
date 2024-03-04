using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.Metrics;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Metrics;
public class UsedFileStorageSpaceMetricCalculatorTests
{
    [Fact]
    public async Task Aggregate_Used_Space_Throws_OverflowException_When_Sum_Wont_Fit_In_A_Long()
    {
        // Arrange
        var repositoryStub = new FileMetadataRepositoryStub(long.MaxValue);
        var metricCalculator = new UsedFileStorageSpaceMetricCalculator(repositoryStub);

        // Act
        var acting = async () => await metricCalculator.CalculateUsage(DateTime.UtcNow, DateTime.UtcNow, "some-address", CancellationToken.None);

        // Assert
        await acting.Should().ThrowAsync<OverflowException>();
    }

    private class FileMetadataRepositoryStub : IFilesRepository
    {
        private readonly long _aggregateUsedSpace;

        public FileMetadataRepositoryStub(long size)
        {
            _aggregateUsedSpace = size;
        }

        public Task<long> AggregateUsedSpace(string uploader, DateTime from, DateTime to, CancellationToken cancellationToken)
        {
            return Task.FromResult(_aggregateUsedSpace);
        }

        public Task<uint> Count(string uploader, DateTime createdAtFrom, DateTime createdAtTo, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

