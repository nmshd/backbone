using System.Data;
using Microsoft.Data.SqlClient;

namespace Enmeshed.Common.Infrastructure.Persistence.Context;
public class MetricStatusesDapperContext
{
    public string ConnectionString = string.Empty;

    public IDbConnection CreateConnection() => new SqlConnection(ConnectionString);
}