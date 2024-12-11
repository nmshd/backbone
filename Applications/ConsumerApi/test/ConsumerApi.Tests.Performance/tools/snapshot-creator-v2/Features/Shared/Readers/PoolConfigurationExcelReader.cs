using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Readers.Base;
using Ganss.Excel;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Readers;

public class PoolConfigurationExcelReader(IExcelReader excelReader) : PoolConfigurationReaderBase, IPoolConfigurationExcelReader
{
    protected override string[] ValidExtensions { get; } = [".xlsx", ".xls"];

    public async Task<PerformanceTestConfiguration> Read(string filePath, string workSheet)
    {
        VerifyFileExtension(filePath);

        var excelMapper = new ExcelMapper(filePath) { SkipBlankRows = true, SkipBlankCells = true, TrackObjects = false };

        await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var poolConfigFromExcel = await excelReader.FetchAsync(workSheet, excelMapper, stream);

        List<PoolConfiguration> identityPoolConfigs = [];
        VerificationConfiguration verificationConfiguration = new();

        var performanceTestConfiguration = new PerformanceTestConfiguration(identityPoolConfigs, verificationConfiguration);

        var materializedPoolConfig = poolConfigFromExcel as List<dynamic> ?? poolConfigFromExcel.ToList();
        if (materializedPoolConfig.Count == 0)
        {
            throw new InvalidOperationException(PERFORMANCE_TEST_CONFIGURATION_EXCEL_FILE_EMPTY);
        }

        if (materializedPoolConfig.First() is not IDictionary<string, object> firstRow)
        {
            throw new InvalidOperationException($"{PERFORMANCE_TEST_CONFIGURATION_FIRST_ROW_MISMATCH} {nameof(IDictionary<string, object>)}");
        }

        verificationConfiguration.TotalNumberOfRelationships = Convert.ToInt32(firstRow[TOTAL_NUMBER_OF_RELATIONSHIPS]);
        verificationConfiguration.TotalAppSentMessages = Convert.ToInt64(firstRow[APP_TOTAL_NUMBER_OF_SENT_MESSAGES]);
        verificationConfiguration.TotalConnectorSentMessages = Convert.ToInt64(firstRow[CONNECTOR_TOTAL_NUMBER_OF_SENT_MESSAGES]);

        foreach (var data in materializedPoolConfig)
        {
            if (data is not IDictionary<string, object> row)
            {
                continue;
            }

            var item = new PoolConfiguration
            {
                Type = (string)row[nameof(PoolConfiguration.Type)],
                Name = (string)row[nameof(PoolConfiguration.Name)],
                Alias = (string)row[nameof(PoolConfiguration.Alias)],
                Amount = Convert.ToInt64(row[nameof(PoolConfiguration.Amount)]),
                NumberOfRelationshipTemplates = Convert.ToInt32(row[nameof(PoolConfiguration.NumberOfRelationshipTemplates)]),
                NumberOfRelationships = Convert.ToInt32(row[nameof(PoolConfiguration.NumberOfRelationships)]),
                NumberOfSentMessages = Convert.ToInt32(row[nameof(PoolConfiguration.NumberOfSentMessages)]),
                NumberOfReceivedMessages = Convert.ToInt32(row[nameof(PoolConfiguration.NumberOfReceivedMessages)]),
                NumberOfDatawalletModifications = Convert.ToInt32(row[nameof(PoolConfiguration.NumberOfDatawalletModifications)]),
                NumberOfDevices = Convert.ToInt32(row[nameof(PoolConfiguration.NumberOfDevices)]),
                NumberOfChallenges = Convert.ToInt32(row[nameof(PoolConfiguration.NumberOfChallenges)])
            };

            identityPoolConfigs.Add(item);
        }

        performanceTestConfiguration.CreateIdentityPoolConfigurations();

        return performanceTestConfiguration;
    }
}
