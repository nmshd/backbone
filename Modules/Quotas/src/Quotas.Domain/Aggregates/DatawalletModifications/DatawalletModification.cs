﻿namespace Backbone.Modules.Quotas.Domain.Aggregates.DatawalletModifications;

public class DatawalletModification : ICreatedAt
{
    public required string Id { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required string CreatedBy { get; set; }
}
