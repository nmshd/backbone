﻿namespace Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types;

public class StartSyncRunRequest
{
    public SyncRunType? Type { get; set; }
    public ushort? Duration { get; set; }
}

public enum SyncRunType
{
    ExternalEventSync = 0,
    DatawalletVersionUpgrade = 1
}
