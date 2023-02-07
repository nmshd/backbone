using System.ComponentModel.DataAnnotations;
using Backbone.Modules.Devices.Application;

namespace Backbone.API.Configuration;

public class DevicesConfiguration
{
    [Required]
    public ApplicationOptions Application { get; set; } = new();

    [Required]
    public InfrastructureConfiguration Infrastructure { get; set; } = new();

    public class InfrastructureConfiguration
    {
        [Required]
        public SqlDatabaseConfiguration SqlDatabase { get; set; } = new();

        [Required]
        public AzureNotificationHubConfiguration AzureNotificationHub { get; set; } = new();

        public class SqlDatabaseConfiguration
        {
            [Required]
            [MinLength(1)]
            public string ConnectionString { get; set; } = string.Empty;
        }

        public class AzureNotificationHubConfiguration
        {
            [Required]
            [MinLength(1)]
            public string ConnectionString { get; set; } = "";

            [Required]
            [MinLength(1)]
            public string HubName { get; set; } = "";
        }

    }
}