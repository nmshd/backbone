using System.Data;
using Microsoft.Data.SqlClient;

namespace Enmeshed.Common.Infrastructure.Persistence.Context;
public class MetricStatusesDapperContext
{
    public MetricStatusesDapperContext()
    {
        Connection = new SqlConnection(ConnectionString);
    }

    public string ConnectionString = string.Empty;
        
    public IDbConnection Connection { get; }
}