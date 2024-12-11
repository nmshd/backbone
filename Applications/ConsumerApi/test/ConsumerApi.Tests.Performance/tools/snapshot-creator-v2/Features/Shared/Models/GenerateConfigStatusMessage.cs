namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

public record GenerateConfigStatusMessage(bool Status, string? PoolConfigurationFolder, string Message, Exception? Exception = null);
