using System.ComponentModel.DataAnnotations;
using Backbone.Modules.Tags.Application;

namespace Backbone.Modules.Tags.ConsumerApi;

public class Configuration
{
    public ApplicationOptions Application { get; set; } = new();

    [Required]
    public InfrastructureConfiguration Infrastructure { get; set; } = new();

    public class InfrastructureConfiguration
    {
    }
}
