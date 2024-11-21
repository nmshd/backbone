namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

public record StatusMessage(bool Status, string Message, Exception? Exception = null);
