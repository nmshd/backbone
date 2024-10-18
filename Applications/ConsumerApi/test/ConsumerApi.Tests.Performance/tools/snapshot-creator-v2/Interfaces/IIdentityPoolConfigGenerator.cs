namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Interfaces;

public interface IIdentityPoolConfigGenerator
{
    Task<bool> VerifyJsonPoolConfig(string excelFile, string workSheetName, string poolConfigJsonFile);
    Task<(bool Status, string Message)> GenerateJsonPoolConfig(string excelFile, string workSheetName);
}
