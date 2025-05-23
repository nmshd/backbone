using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;

namespace Backbone.AdminCli.Configuration;

public class AdminCliConfiguration
{
    [Required]
    public AdminInfrastructureConfiguration Infrastructure { get; set; } = new();

    public class AdminInfrastructureConfiguration
    {
        [Required]
        public EventBusConfiguration EventBus { get; set; } = new();

        [Required]
        public DatabaseConfiguration SqlDatabase { get; set; } = new();
    }
}
