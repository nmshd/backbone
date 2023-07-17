using System.Data;
using Dapper;
using Enmeshed.BuildingBlocks.Domain;

namespace Enmeshed.Common.Infrastructure.Persistence.Context;
internal class MetricKeyTypeHandler : SqlMapper.TypeHandler<MetricKey>
{
    public override MetricKey Parse(object value)
    {
        return new MetricKey((string)value);
    }

    public override void SetValue(IDbDataParameter parameter, MetricKey value)
    {
        parameter.Value = value.ToString();
    }
}
