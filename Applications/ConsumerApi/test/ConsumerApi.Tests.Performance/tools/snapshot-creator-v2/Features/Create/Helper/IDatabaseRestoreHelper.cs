﻿using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Helper;

public interface IDatabaseRestoreHelper
{
    Task<StatusMessage> RestoreCleanDatabase(string scriptPath);
}
