using System.Diagnostics;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Models;

public record ProcessParams(Process Process, string? Output, bool HasError);
