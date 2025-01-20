namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

public record DatabaseResult(bool Status, string Message, bool IsError = false, Exception? Exception = null) : StatusMessage(Status, Message, Exception);
