using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Enmeshed.Common.Infrastructure.Persistence.Context;
public class DapperContext
{
    private readonly string _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        var sqlDatabaseSection = configuration.GetSection("Modules:Quotas:Infrastructure:SqlDatabase");

        switch (sqlDatabaseSection.GetSection("Provider").Value)
        {
            case "Postgres":
            case "SqlServer":
                break;
            default:
                throw new Exception($"Unsupported database provider passed.");
        }

        var connectionStringSection = configuration.GetSection("Modules:Quotas:Infrastructure:SqlDatabase:ConnectionString");
        
        _connectionString = connectionStringSection.Value ?? throw new Exception("Could not parse connection string.");
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}