using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Enmeshed.Common.Infrastructure.Persistence.Context;
public class MetricStatusesDapperContext
{
    private const string SQLSERVER = "SqlServer";
    private const string POSTGRES = "Postgres";
    public MetricStatusesDapperContext(IOptions<MetricStatusesDapperContextOptions> options)
    {
        switch (options.Value.Provider)
        {
            case SQLSERVER:
                Connection = new SqlConnection(options.Value.ConnectionString);
                break;
            case POSTGRES:
                Connection = new NpgsqlConnection(options.Value.ConnectionString);
                break;
            default:
                throw new Exception($"Unsupported database provider: {options.Value.Provider}");
        }

        SqlMapper.AddTypeHandler(new MetricKeyTypeHandler());
    }
    public IDbConnection Connection { get; }
}

public class MetricStatusesDapperContextOptions
{
    public string ConnectionString = string.Empty;
    public string Provider = string.Empty;
}