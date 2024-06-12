namespace Backbone.Modules.Quotas.Domain.Aggregates.StartedDeletionProcesses;
public class StartedDeletionProcess : ICreatedAt
{
    public required string Id { get; set; }
    public required string From { get; set; }
    public required DateTime CreatedAt { get; set; }
}
