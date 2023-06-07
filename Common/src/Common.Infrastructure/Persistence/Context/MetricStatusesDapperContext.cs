using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Enmeshed.Common.Infrastructure.Persistence.Context;
public class MetricStatusesDapperContext
{
    public MetricStatusesDapperContext(IOptions<MetricStatusesDapperContextOptions> options)
    {
        Connection = new SqlConnection(options.Value.ConnectionString);
    }
    public IDbConnection Connection { get; }
}

public class MetricStatusesDapperContextOptions
{
    public string ConnectionString = string.Empty;
}