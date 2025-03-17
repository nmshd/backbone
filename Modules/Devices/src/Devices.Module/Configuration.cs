using System.ComponentModel.DataAnnotations;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;

namespace Backbone.Modules.Devices.Module;

public class InfrastructureConfiguration
{
    [Required]
    public SqlDatabaseConfiguration SqlDatabase { get; set; } = new();

    [Required]
    public PushNotificationOptions PushNotifications { get; set; } = new();

    public class SqlDatabaseConfiguration
    {
        [Required]
        [MinLength(1)]
        [RegularExpression("SqlServer|Postgres")]
        public string Provider { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public string ConnectionString { get; set; } = string.Empty;

        [Required]
        public bool EnableHealthCheck { get; set; } = true;
    }
}
