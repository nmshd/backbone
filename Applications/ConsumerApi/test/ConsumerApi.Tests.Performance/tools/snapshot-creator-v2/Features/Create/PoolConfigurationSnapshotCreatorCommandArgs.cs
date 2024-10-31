namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create;

public record PoolConfigurationSnapshotCreatorCommandArgs(
    string BaseAddress,
    string ClientId,
    string ClientSecret,
    string JsonFilePath);
