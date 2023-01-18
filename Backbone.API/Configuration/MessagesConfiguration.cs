using Messages.Application;

namespace Backbone.API.Configuration;

public class MessagesConfiguration
{
    public ApplicationOptions Application { get; set; } = new();
    public InfrastructureConfiguration Infrastructure { get; set; } = new();

    public class InfrastructureConfiguration
    {
        public SqlDatabaseConfiguration SqlDatabase { get; set; } = new();

        public BlobStorageConfiguration BlobStorage { get; set; } = new();

        public class BlobStorageConfiguration
        {
            public string ConnectionInfo { get; set; } = string.Empty;
            public string CloudProvider { get; set; } = string.Empty;
            public string ContainerName { get; set; } = string.Empty;
        }

        public class SqlDatabaseConfiguration
        {
            public string ConnectionString { get; set; } = string.Empty;
        }
    }
}