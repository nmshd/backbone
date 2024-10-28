using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;
using Ganss.Excel;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Readers;

public class PerformanceTestConfigurationExcelReader
{
    public async Task<PerformanceTestConfiguration> Read(string filePath, string workSheet)
    {
        var excelMapper = new ExcelMapper(filePath) { SkipBlankRows = true, SkipBlankCells = true, TrackObjects = false };

        await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var poolConfigFromExcel = await excelMapper.FetchAsync(stream, workSheet);


        List<IdentityPoolConfiguration> identityPoolConfigs = [];
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

            var item = new IdentityPoolConfiguration
            {
                Type = (string)row[nameof(IdentityPoolConfiguration.Type)],
                Name = (string)row[nameof(IdentityPoolConfiguration.Name)],
                Alias = (string)row[nameof(IdentityPoolConfiguration.Alias)],
                Amount = Convert.ToInt64(row[nameof(IdentityPoolConfiguration.Amount)]),
                NumberOfRelationshipTemplates = Convert.ToInt32(row[nameof(IdentityPoolConfiguration.NumberOfRelationshipTemplates)]),
                NumberOfRelationships = Convert.ToInt32(row[nameof(IdentityPoolConfiguration.NumberOfRelationships)]),
                NumberOfSentMessages = Convert.ToInt32(row[nameof(IdentityPoolConfiguration.NumberOfSentMessages)]),
                NumberOfReceivedMessages = Convert.ToInt32(row[nameof(IdentityPoolConfiguration.NumberOfReceivedMessages)]),
                NumberOfDatawalletModifications = Convert.ToInt32(row[nameof(IdentityPoolConfiguration.NumberOfDatawalletModifications)]),
                NumberOfDevices = Convert.ToInt32(row[nameof(IdentityPoolConfiguration.NumberOfDevices)]),
                NumberOfChallenges = Convert.ToInt32(row[nameof(IdentityPoolConfiguration.NumberOfChallenges)])
            };

            identityPoolConfigs.Add(item);
        }

        return performanceTestConfiguration;
    }
}
