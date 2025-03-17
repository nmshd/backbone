using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.EventBus;

namespace Backbone.AdminCli.Configuration;

public class AdminCliConfiguration
{
    [Required]
    public AdminInfrastructureConfiguration Infrastructure { get; set; } = new();

    public class AdminInfrastructureConfiguration
    {
        [Required]
        public EventBusOptions EventBus { get; set; } = new();
    }
}
