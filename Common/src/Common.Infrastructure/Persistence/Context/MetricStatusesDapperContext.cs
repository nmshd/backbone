using System.Data;
using Microsoft.Data.SqlClient;

namespace Enmeshed.Common.Infrastructure.Persistence.Context;
public class MetricStatusesDapperContext
{
    public string ConnectionString;

    public MetricStatusesDapperContext()
    {}

    public IDbConnection CreateConnection() => new SqlConnection(ConnectionString);
}