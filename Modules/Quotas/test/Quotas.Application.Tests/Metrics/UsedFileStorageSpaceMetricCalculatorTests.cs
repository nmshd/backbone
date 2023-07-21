using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.FileMetadata;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Metrics;
public class UsedFileStorageSpaceMetricCalculatorTests
{
    [Fact]
    public async Task Aggregate_Used_Space_Throws_OverflowException_When_Sum_Wont_Fit_In_A_Long()
    {
        // Arrange
        var repositoryStub = new FileMetadataRepositoryStub(new List<FileMetadata>() { new() { CipherSize = 9223372036854775806 }, new() { CipherSize = 9223372036854775806 } });

        // Act
        var acting = async () => await repositoryStub.AggregateUsedSpace("", DateTime.UtcNow, DateTime.UtcNow, CancellationToken.None);

        // Assert
        await acting.Should().ThrowAsync<OverflowException>();
    }

    private class FileMetadataRepositoryStub : IFilesRepository
    {
        private readonly IEnumerable<FileMetadata> _filesInRepository;

        public FileMetadataRepositoryStub(IEnumerable<FileMetadata> files)
        {
            _filesInRepository = files;
        }

        public Task<long> AggregateUsedSpace(string uploader, DateTime from, DateTime to, CancellationToken cancellationToken)
        {
            return Task.FromResult(_filesInRepository.Sum(f => f.CipherSize));
        }

        public Task<uint> Count(string uploader, DateTime createdAtFrom, DateTime createdAtTo, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

