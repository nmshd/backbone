﻿namespace Backbone.Modules.Quotas.Domain.Aggregates.StartedDeletionProcesses;

public class IdentityDeletionProcesses : ICreatedAt
{
    public required string Id { get; set; }
    public required DeletionProcessStatus Status { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime DeletionStartedAt { get; set; }
}

public enum DeletionProcessStatus
{
    WaitingForApproval = 0,
    Approved = 1,
    Cancelled = 2,
    Rejected = 3,
    Deleting = 10
}
