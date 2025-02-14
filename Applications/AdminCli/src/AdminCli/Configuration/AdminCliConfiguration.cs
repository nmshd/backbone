using System.ComponentModel.DataAnnotations;
using Backbone.Infrastructure.EventBus;

namespace Backbone.AdminCli.Configuration;

public class AdminCliConfiguration
{
    [Required]
    public AdminInfrastructureConfiguration Infrastructure { get; set; } = new();

    [Required]
    public ModulesConfiguration Modules { get; set; } = new();

    public class AdminInfrastructureConfiguration
    {
        [Required]
        public EventBusConfiguration EventBus { get; set; } = new();
    }

    public class ModulesConfiguration
    {
        [Required]
        public DevicesConfiguration Devices { get; set; } = new();
    }
}
