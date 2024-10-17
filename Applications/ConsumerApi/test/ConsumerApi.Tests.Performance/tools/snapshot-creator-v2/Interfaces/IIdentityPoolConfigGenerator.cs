namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Interfaces;

public interface IIdentityPoolConfigGenerator
{
    Task<bool> VerifyJsonPoolConfig(string excelFile, string workSheetName, string poolConfigJsonFile);
}