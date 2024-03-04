using System.ComponentModel.DataAnnotations;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus;

public class HandlerRetryBehavior
{
    [Range(1, int.MaxValue)]
    public int NumberOfRetries { get; set; }

    [Range(1, int.MaxValue)]
    public int MinimumBackoff { get; set; }

    [Range(1, int.MaxValue)]
    public int MaximumBackoff { get; set; }
}
