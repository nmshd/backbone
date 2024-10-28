using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Readers;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Writers;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Commands;

public class PoolConfigurationJsonGeneratorCommand : ICommand<PoolConfigurationJsonGeneratorCommandArgs, (bool Status, string Message)>
{
    private readonly ILogger<PoolConfigurationJsonGeneratorCommand> _logger;
    private readonly PoolConfigurationJsonWriter _poolConfigurationJsonWriter;
    private readonly PerformanceTestConfigurationExcelReader _performanceTestConfigurationExcelReader;

    public PoolConfigurationJsonGeneratorCommand(
        ILogger<PoolConfigurationJsonGeneratorCommand> logger,
        PoolConfigurationJsonWriter poolConfigurationJsonWriter,
        PerformanceTestConfigurationExcelReader performanceTestConfigurationExcelReader)
    {
        _logger = logger;
        _poolConfigurationJsonWriter = poolConfigurationJsonWriter;
        _performanceTestConfigurationExcelReader = performanceTestConfigurationExcelReader;
    }

    public async Task<(bool Status, string Message)> Execute(PoolConfigurationJsonGeneratorCommandArgs parameter)
    {
        var poolConfigFromExcel = await _performanceTestConfigurationExcelReader.Read(parameter.ExcelFilePath, parameter.WorkSheetName);
        var result = await _poolConfigurationJsonWriter.Write(poolConfigFromExcel, parameter.WorkSheetName);

        return result;
    }
}

public record PoolConfigurationJsonGeneratorCommandArgs(string ExcelFilePath, string WorkSheetName);

public interface ICommand<in TArgs, TResult> where TArgs : class
{
    Task<TResult> Execute(TArgs parameter);
}
