using System.ComponentModel.DataAnnotations;

namespace Backbone.Modules.Quotas.Module;

public class InfrastructureConfiguration
{
    [Required]
    public SqlDatabaseConfiguration SqlDatabase { get; set; } = new();

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
